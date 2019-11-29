using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlgoritmosEDII
{
    public class LZW
    {
        public static Queue<string> CadenaTexto = new Queue<string>();
        public static void Comprimir(string Ruta)
        {
            Dictionary<string, int> Diccionario = new Dictionary<string, int>();
            Leer(Ruta);
            LlenarDiccionario(Diccionario, CadenaTexto);
            FileInfo Informacion = new FileInfo(Ruta);
            string RutaSalida = Informacion.Name.Replace(".txt", ".lzw");
            Compresion(Diccionario, RutaSalida);

        }
        public static void Leer(string Ruta)
        {
            const int LongitudBuffer = 1000;
            byte[] Buffer = new byte[LongitudBuffer];
            using (var Archivo = new FileStream(Ruta, FileMode.Open))
            {
                using (var Lector = new BinaryReader(Archivo))
                {
                    while (Lector.BaseStream.Position != Lector.BaseStream.Length)
                    {
                        Buffer = Lector.ReadBytes(LongitudBuffer);
                        string[] CodigoCaracter = new string[Buffer.Length];
                        char RellenarByte = '0';
                        foreach (var Byte in Buffer)
                        {
                            int numero = Convert.ToInt32(Byte);
                            string NumeroBin = Convert.ToString(numero, 2);
                            string CodigoNumero = NumeroBin.PadLeft(8, RellenarByte);
                            CadenaTexto.Enqueue(CodigoNumero);

                        }
                    }
                }
            }
        }
        public static void LlenarDiccionario(Dictionary<string, int> Diccionario, Queue<string> Cadena)
        {
            foreach (var Elemento in Cadena)
            {
                if (Diccionario.Count == 0)
                {
                    Diccionario.Add(Elemento, Diccionario.Count + 1);
                }
                else
                {
                    if (Diccionario.ContainsKey(Elemento) == false)
                    {
                        Diccionario.Add(Elemento, Diccionario.Count + 1);
                    }
                }
            }
        }
        public static void Compresion(Dictionary<string, int> Diccionario, string RutaSalida)
        {
            GuardarDiccionario(Diccionario, RutaSalida);
            CompresionIterativo(CadenaTexto, "", Diccionario, RutaSalida);
        }
        public static void GuardarDiccionario(Dictionary<string, int> Diccionario, string Ruta)
        {
            StreamWriter Escribir = new StreamWriter(Ruta);
            foreach (var Elemento in Diccionario)
            {
                char temp = Convert.ToChar(Elemento.Value);
                Escribir.Write(Elemento.Key + temp);
            }
            Escribir.Write("-_ ");
            Escribir.Close();
        }
        public static void CompresionIterativo(Queue<string> Cadena, string CaracterActual, Dictionary<string, int> Diccionario, string RutaSalida)
        {
            string CaracterSiguiente;
            string Actual_y_Siguiente;
            byte[] ByteAEscribir = new byte[3];
            int Cociente, Residuo;
            while (Cadena.Count > 0)
            {
                CaracterSiguiente = Cadena.Dequeue();
                Actual_y_Siguiente = CaracterActual + CaracterSiguiente;
                if (Diccionario.ContainsKey(Actual_y_Siguiente))
                {
                    CaracterActual = Actual_y_Siguiente;
                }
                else
                {
                    Diccionario.Add(Actual_y_Siguiente, Diccionario.Count + 1);
                    if (Diccionario[CaracterActual] < 256)
                    {
                        ByteAEscribir[0] = Convert.ToByte(0);
                        ByteAEscribir[1] = Convert.ToByte(0);
                        ByteAEscribir[2] = Convert.ToByte(Diccionario[CaracterActual]);
                    }
                    if (Diccionario[CaracterActual] > 255 && Diccionario[CaracterActual] < 65536)
                    {
                        ByteAEscribir[0] = Convert.ToByte(0);
                        Cociente = Diccionario[CaracterActual] / 256;
                        ByteAEscribir[1] = Convert.ToByte(Cociente);
                        Residuo = Diccionario[CaracterActual] % 256;
                        ByteAEscribir[2] = Convert.ToByte(Residuo);
                    }
                    if (Diccionario[CaracterActual] > 65535)
                    {
                        Cociente = Diccionario[CaracterActual] / 65536;
                        ByteAEscribir[0] = Convert.ToByte(Cociente);
                        Cociente = Diccionario[CaracterActual] % 65536;
                        ByteAEscribir[1] = Convert.ToByte(Cociente / 256);
                        Residuo = Cociente % 256;
                        ByteAEscribir[2] = Convert.ToByte(Residuo);
                    }
                    EscribirBytes(RutaSalida, ByteAEscribir);
                    CaracterActual = CaracterSiguiente;
                }
            }
            {
                if (Diccionario[CaracterActual] < 256)
                {
                    ByteAEscribir[0] = Convert.ToByte(0);
                    ByteAEscribir[1] = Convert.ToByte(0);
                    ByteAEscribir[2] = Convert.ToByte(Diccionario[CaracterActual]);
                }
                if (Diccionario[CaracterActual] > 255 && Diccionario[CaracterActual] < 65536)
                {
                    ByteAEscribir[0] = Convert.ToByte(0);
                    Cociente = Diccionario[CaracterActual] / 256;
                    ByteAEscribir[1] = Convert.ToByte(Cociente);
                    Residuo = Diccionario[CaracterActual] % 256;
                    ByteAEscribir[2] = Convert.ToByte(Residuo);
                }
                if (Diccionario[CaracterActual] > 65535)
                {
                    Cociente = Diccionario[CaracterActual] / 65536;
                    ByteAEscribir[0] = Convert.ToByte(Cociente);
                    Cociente = Diccionario[CaracterActual] % 65536;
                    ByteAEscribir[1] = Convert.ToByte(Cociente / 256);
                    Residuo = Cociente % 256;
                    ByteAEscribir[2] = Convert.ToByte(Residuo);
                }
                EscribirBytes(RutaSalida, ByteAEscribir);
            }
        }
        public static void EscribirBytes(string Ruta, byte[] ByteAEscribir)
        {
            using (var Stream = new FileStream(Ruta, FileMode.Append))
            {
                Stream.Write(ByteAEscribir, 0, ByteAEscribir.Length);
            }
        }
        public static void Descomprimir(string Ruta)
        {
            List<byte> Lista = new List<byte>();
            List<byte> ListaDiccionario = new List<byte>();
            Queue<int> DatosCompresos = new Queue<int>();
            Dictionary<int, string> Diccionario = new Dictionary<int, string>();
            LecturaDescompresion(Ruta, Lista);
            SepararDiccionario(Lista, ListaDiccionario);
            ArmarDiccionarioInicial(ListaDiccionario, Diccionario);
            DepurarData(DatosCompresos, Lista);
            FileInfo Informacion = new FileInfo(Ruta);
            string RutaSalida = Informacion.Name.Replace(".lzw", ".txt");
            DescompresionIterativo(DatosCompresos, Diccionario, RutaSalida);
        }
        public static void LecturaDescompresion(string Ruta, List<byte> Lista)
        {
            const int LongitudBuffer = 1000;
            var Buffer = new byte[LongitudBuffer];
            using (var Archivo = new FileStream(Ruta, FileMode.Open))
            {
                using (var Lector = new BinaryReader(Archivo))
                {
                    while (Lector.BaseStream.Position != Lector.BaseStream.Length)
                    {
                        Buffer = Lector.ReadBytes(LongitudBuffer);
                        foreach (var Elemento in Buffer)
                        {
                            Lista.Add(Elemento);
                        }
                    }
                }
            }
        }
        public static void SepararDiccionario(List<byte> Lista, List<byte> ListaDiccionario)
        {
            int IndiceDiccionario = 0;
            for (int i = 0; i < Lista.Count; i++)
            {
                if (Lista[i] == 45 && Lista[i + 1] == 95 && Lista[i + 2] == 32)
                {
                    IndiceDiccionario = i;
                    break;
                }
            }
            for (int i = 0; i < IndiceDiccionario; i++)
            {
                ListaDiccionario.Add(Lista[i]);
            }
            Lista.RemoveRange(0, ListaDiccionario.Count + 3);
        }
        public static void ArmarDiccionarioInicial(List<byte> ListaDiccionario, Dictionary<int, string> Diccionario)
        {
            string Valor1 = "", Valor2 = "", Valor3 = "", Valor4 = "", Valor5 = "", Valor6 = "", Valor7 = "", Valor8 = "", Valor = "";

            int Llave;
            byte[] ArrayDiccionario = ListaDiccionario.ToArray();
            string Valores = Encoding.UTF8.GetString(ArrayDiccionario);
            char[] ArregloCaracteres = Valores.ToCharArray();
            for (int i = 0; i < ArregloCaracteres.Length; i += 9)
            {
                Valor1 = Convert.ToString(ArregloCaracteres[i]);
                Valor2 = Convert.ToString(ArregloCaracteres[i + 1]);
                Valor3 = Convert.ToString(ArregloCaracteres[i + 2]);
                Valor4 = Convert.ToString(ArregloCaracteres[i + 3]);
                Valor5 = Convert.ToString(ArregloCaracteres[i + 4]);
                Valor6 = Convert.ToString(ArregloCaracteres[i + 5]);
                Valor7 = Convert.ToString(ArregloCaracteres[i + 6]);
                Valor8 = Convert.ToString(ArregloCaracteres[i + 7]);
                Valor = Valor1 + Valor2 + Valor3 + Valor4 + Valor5 + Valor6 + Valor7 + Valor8;
                //Valor = Convert.ToString(ArregloCaracteres[i] + ArregloCaracteres[i + 1] + ArregloCaracteres[i + 2] + ArregloCaracteres[i + 3] + ArregloCaracteres[i + 4] + ArregloCaracteres[i + 5] + ArregloCaracteres[i + 6] + ArregloCaracteres[i + 7]);
                Llave = Convert.ToInt32(ArregloCaracteres[i + 8]);
                Diccionario.Add(Llave, Valor);
            }
            ListaDiccionario.Clear();
        }
        public static void DepurarData(Queue<int> DatosCompresos, List<byte> Lista)
        {
            int Encolar;
            for (int i = 0; i < Lista.Count; i += 3)
            {
                Encolar = (Lista[i] * 65536) + (Lista[i + 1] * 256) + (Lista[i + 2]);
                DatosCompresos.Enqueue(Encolar);
            }
            Lista.Clear();
        }
        public static void DescompresionIterativo(Queue<int> DatosCompresos, Dictionary<int, string> Diccionario, string RutaSalida)
        {
            List<byte> CadenaSalida = new List<byte>();
            int ClaveAnterior, ClaveNuevo;
            string Cadena, Caracter;
            int Numero;
            byte NumeroB;
            ClaveAnterior = DatosCompresos.Dequeue();
            Caracter = Diccionario[ClaveAnterior];
            Numero = Convert.ToInt32(Caracter, 2);
            NumeroB = Convert.ToByte(Numero);
            CadenaSalida.Add(NumeroB);
            while (DatosCompresos.Count > 0)
            {
                ClaveNuevo = DatosCompresos.Dequeue();
                if (Diccionario.ContainsKey(ClaveNuevo) == false)
                {
                    Cadena = Diccionario[ClaveAnterior];
                    Cadena = Cadena + Caracter;
                }
                else
                {
                    Cadena = Diccionario[ClaveNuevo];
                }
                if (Cadena.Length == 8)
                {
                    Numero = Convert.ToInt32(Cadena, 2);
                    NumeroB = Convert.ToByte(Numero);
                    CadenaSalida.Add(NumeroB);
                }
                else
                {
                    char[] Masde1 = Cadena.ToCharArray();
                    for (int i = 0; i < Masde1.Length; i += 8)
                    {
                        string Bit1 = Masde1[i].ToString();
                        string Bit2 = Masde1[i + 1].ToString();
                        string Bit3 = Masde1[i + 2].ToString();
                        string Bit4 = Masde1[i + 3].ToString();
                        string Bit5 = Masde1[i + 4].ToString();
                        string Bit6 = Masde1[i + 5].ToString();
                        string Bit7 = Masde1[i + 6].ToString();
                        string Bit8 = Masde1[i + 7].ToString();
                        string Byte = Bit1 + Bit2 + Bit3 + Bit4 + Bit5 + Bit6 + Bit7 + Bit8;
                        Numero = Convert.ToInt32(Byte, 2);
                        NumeroB = Convert.ToByte(Numero);
                        CadenaSalida.Add(NumeroB);
                    }
                }
                Caracter = Cadena.Substring(0, 8);
                Diccionario.Add(Diccionario.Count + 1, Diccionario[ClaveAnterior] + Caracter);
                ClaveAnterior = ClaveNuevo;
            }
            byte[] ArregloFinal = CadenaSalida.ToArray();
            using (var Stream = new FileStream(RutaSalida, FileMode.Append))
            {
                Stream.Write(ArregloFinal, 0, ArregloFinal.Length);
            }
        }
    }
}
