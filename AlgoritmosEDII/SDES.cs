using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgoritmosEDII
{
    public class SDES
    {

        public SDES()
        {
            LeerArchivoPermutaciones();
        }
        Queue<byte> TextoEntrada = new Queue<byte>(); //Se almacena todo el texto del archivo en una cola para evitar ovverflow
        Queue<byte> ColaTextoCifrado = new Queue<byte>(); //Texto resultado del cifrado, se escribirá en el archivo
        string[,] Sbox1 = { //Definicion de la primera sbox
                                    { "01", "00", "11", "10"},
                                    { "11", "10", "01", "00"},
                                    { "00", "10", "01", "11"},
                                    { "11", "01", "11", "10"}};//Sbox No. 1
        string[,] Sbox2 = { //Definicion de la segunda sbox
                                    { "00", "01", "10", "11"},
                                    { "10", "00", "01", "11"},
                                    { "11", "00", "01", "00"},
                                    { "10", "01", "00", "11"}};//Sbox No. 2

        List<string> PermutacionesDelArchivo = new List<string>(); //Variables para la lectura del archivo
        int[] Permutar10 = new int[10];
        int[] PermutacionYseleccion = new int[8];
        int[] Permutar4 = new int[4];
        int[] PermutarYExpandir = new int[8];
        int[] PermutacionInici = new int[8];
        int[] PermutarInversa = new int[8];
        void LeerArchivoPermutaciones()
        {

            StreamReader reader = new StreamReader("Permutaciones.txt");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                PermutacionesDelArchivo.Add(line);
            }
            reader.Close();

            Permutar10 = PermutacionesDelArchivo[0].Split(',').Select(Int32.Parse).ToArray();
            PermutacionYseleccion = PermutacionesDelArchivo[1].Split(',').Select(Int32.Parse).ToArray();
            Permutar4 = PermutacionesDelArchivo[2].Split(',').Select(Int32.Parse).ToArray();
            PermutarYExpandir = PermutacionesDelArchivo[3].Split(',').Select(Int32.Parse).ToArray();
            PermutacionInici = PermutacionesDelArchivo[4].Split(',').Select(Int32.Parse).ToArray();
            PermutarInversa = PermutacionesDelArchivo[5].Split(',').Select(Int32.Parse).ToArray();
        }
        //Metodos públicos para hacer las operaciones de encriptar y desencriptar
        public string Encriptar(int key, string RutaEntrada)
        {
            char[] Key1 = new char[8];
            char[] Key2 = new char[8];
            ColaTextoCifrado.Clear(); //Limpiar estructtturas de datos para evitar errores
            TextoEntrada.Clear();//Limpiar estructtturas de datos para evitar errores
            byte TextoCifrado = 0; //Se almacenara el texto cifrado a escribir en un archivo de texto
                                   //LeerArchivo(TextoEntrada, RutaEntrada);
            LeerString(TextoEntrada, RutaEntrada);
            string Llave = Convert.ToString(key, 2);
            Llave = Llave.PadLeft(10, '0'); //Convertir el int ingresado 10 bits
            DefinirLlaves(Llave, ref Key1, ref Key2);
            while (TextoEntrada.Count > 0)
            {
                CifrarCaracteres(TextoEntrada.Dequeue(), Key1, Key2, ref TextoCifrado); //Cifrar cada caracter en el texto
                ColaTextoCifrado.Enqueue(TextoCifrado);
            }
            return TextoCifra(ColaTextoCifrado);
        }
        public void LeerString(Queue<byte> ColaChar, string Texto)
        {
            byte[] ArregloEntrada = Encoding.UTF8.GetBytes(Texto);
            foreach (char Caracter in ArregloEntrada)
            {
                ColaChar.Enqueue(Convert.ToByte(Caracter));
            }
        }
        public string TextoCifra(Queue<byte> Texto)
        {
            string Resultado = "";
            while (Texto.Count > 0)
            {
                int Codigo = Convert.ToInt32(Texto.Dequeue());
                string Letra = Convert.ToString(Codigo);
                Resultado += Letra + ",";
            }
            int u = 0;
            return Resultado;
        }
        public string TextoEnviado(Queue<byte> Mensaje)
        {
            byte[] MensajeArreglo = Mensaje.ToArray();
            string Salida = Encoding.UTF8.GetString(MensajeArreglo);
            return Salida;
        }
        public string Desencriptar(int key, string Texto)
        {

            char[] Key1 = new char[8];
            char[] Key2 = new char[8];
            ColaTextoCifrado.Clear(); //Limpiar estructtturas de datos para evitar errores
            TextoEntrada.Clear(); //Limpiar estructtturas de datos para evitar errores
            byte TextoCifrado = 0; //Se almacenara el texto cifrado a escribir en un archivo de texto
            //LeerArchivo(TextoEntrada, RutaEntrada);
            //LeerString(TextoEntrada, Texto);
            Queue<byte> BytesEnviados = LeerCifrado(Texto);
            string Llave = Convert.ToString(key, 2);
            Llave = Llave.PadLeft(10, '0'); //Convertir el int ingresado 10 bits
            DefinirLlaves(Llave, ref Key1, ref Key2);
            while (BytesEnviados.Count > 0)
            {
                CifrarCaracteres(BytesEnviados.Dequeue(), Key2, Key1, ref TextoCifrado); //Cifrar cada caracter en el texto
                ColaTextoCifrado.Enqueue(TextoCifrado);
            }
            return TextoDescifra(ColaTextoCifrado);
        }
        public string TextoDescifra(Queue<byte> ColaTextoDescifrado)
        {
            byte[] MensajeEscrito = ColaTextoCifrado.ToArray();
            string Mensaje = Encoding.UTF8.GetString(MensajeEscrito);
            return Mensaje;
        }
        public Queue<byte> LeerCifrado(string Texto)
        {
            Queue<byte> BytesMensaje = new Queue<byte>();
            string[] Bytes = Texto.Split(',');
            for (int i = 0; i < Bytes.Length - 1; i++)
            {
                BytesMensaje.Enqueue(Convert.ToByte(Bytes[i]));
            }
            return BytesMensaje;
        }

        //DEFINICION DE LAS PERMUTACIONES
        char[] Permutacion10(string Numero) //Metodo para permutacion de 10
        {
            char[] LlaveAEncriptar = Numero.ToCharArray();
            char[] Salida = new char[10]; //El resultado despues de realizar la permutación de 10
                                          //Generacion aleatoria
            Salida[0] = LlaveAEncriptar[Permutar10[0]]; //Generacion aleatoria
            Salida[1] = LlaveAEncriptar[Permutar10[1]];
            Salida[2] = LlaveAEncriptar[Permutar10[2]];
            Salida[3] = LlaveAEncriptar[Permutar10[3]];
            Salida[4] = LlaveAEncriptar[Permutar10[4]];
            Salida[5] = LlaveAEncriptar[Permutar10[5]];
            Salida[6] = LlaveAEncriptar[Permutar10[6]];
            Salida[7] = LlaveAEncriptar[Permutar10[7]];
            Salida[8] = LlaveAEncriptar[Permutar10[8]];
            Salida[9] = LlaveAEncriptar[Permutar10[9]];
            return Salida;
        }
        char[] PermutarYseleccionar(char[] Entrada) //Metodo que da como resultado un vector de 8 elementos
        {
            char[] Resultado = new char[8]; //Se almacena el resultado de la permutacion de 8
            Resultado[0] = Entrada[PermutacionYseleccion[0]];
            Resultado[1] = Entrada[PermutacionYseleccion[1]];
            Resultado[2] = Entrada[PermutacionYseleccion[2]];
            Resultado[3] = Entrada[PermutacionYseleccion[3]];
            Resultado[4] = Entrada[PermutacionYseleccion[4]];
            Resultado[5] = Entrada[PermutacionYseleccion[5]];
            Resultado[6] = Entrada[PermutacionYseleccion[6]];
            Resultado[7] = Entrada[PermutacionYseleccion[7]];
            return Resultado;
        }
        char[] Permutacion4(char[] Entrada) //Metodo que da como resultado un vector de 4 elementos
        {
            char[] Resultado = new char[4]; //Se almacena el resultado de la permutacion de 4
            Resultado[0] = Entrada[Permutar4[0]];
            Resultado[1] = Entrada[Permutar4[1]];
            Resultado[2] = Entrada[Permutar4[2]];
            Resultado[3] = Entrada[Permutar4[3]];
            return Resultado;
        }
        char[] ExpandirYPermutar(char[] Entrada) //Recibe un vector con 4 bits y los expande a 8
        {
            char[] Resultado = new char[8]; //Se almacena el resultado de la permutacion de 8
            //Primera parte
            Resultado[0] = Entrada[PermutarYExpandir[0]];
            Resultado[1] = Entrada[PermutarYExpandir[1]];
            Resultado[2] = Entrada[PermutarYExpandir[2]];
            Resultado[3] = Entrada[PermutarYExpandir[3]];
            //segunda parte
            Resultado[4] = Entrada[PermutarYExpandir[4]];
            Resultado[5] = Entrada[PermutarYExpandir[5]];
            Resultado[6] = Entrada[PermutarYExpandir[6]];
            Resultado[7] = Entrada[PermutarYExpandir[7]];
            return Resultado;
        }
        char[] PermutacionInicial(char[] Entrada) //Metodo que Realiza la permutacion Inicial
        {
            char[] Resultado = new char[8]; //Se almacena el resultado de la permutacion de 8
            Resultado[0] = Entrada[PermutacionInici[0]];
            Resultado[1] = Entrada[PermutacionInici[1]];
            Resultado[2] = Entrada[PermutacionInici[2]];
            Resultado[3] = Entrada[PermutacionInici[3]];
            Resultado[4] = Entrada[PermutacionInici[4]];
            Resultado[5] = Entrada[PermutacionInici[5]];
            Resultado[6] = Entrada[PermutacionInici[6]];
            Resultado[7] = Entrada[PermutacionInici[7]];
            return Resultado;
        }
        char[] PermutacionInversa(char[] Entrada) //Metodo que Realiza la permutacion inversa de la permutacion inicial
        {
            char[] Resultado = new char[8]; //Se almacena el resultado de la permutacion inversa de la permutacion inicial
            Resultado[0] = Entrada[PermutarInversa[0]];
            Resultado[1] = Entrada[PermutarInversa[1]];
            Resultado[2] = Entrada[PermutarInversa[2]];
            Resultado[3] = Entrada[PermutarInversa[3]];
            Resultado[4] = Entrada[PermutarInversa[4]];
            Resultado[5] = Entrada[PermutarInversa[5]];
            Resultado[6] = Entrada[PermutarInversa[6]];
            Resultado[7] = Entrada[PermutarInversa[7]];
            return Resultado;
        }

        //METODOS LEFT SHIFTS
        char[] LeftShift1(char[] Entrada5) //Se recibe originalmente un byte de 10 bits a los que hay que hacer leftshift1
        {
            char[] Salida = new char[5];
            char PrimerValor = Entrada5[0]; //Variable auxiliar para almacenar el primer bit
            Salida[0] = Entrada5[1];
            Salida[1] = Entrada5[2];
            Salida[2] = Entrada5[3];
            Salida[3] = Entrada5[4];
            Salida[4] = PrimerValor;
            return Salida;
        }
        char[] LeftShift2(char[] Entrada5) //Se recibe originalmente un byte de 5 bits a los que hay que hacer leftshift2
        {
            char[] Salida = new char[5];
            char PrimerValor = Entrada5[0]; //Variable auxiliar para almacenar el primer bit
            char SegundoValor = Entrada5[1]; //Variable auxiliar para almacenar el segundo bit
            Salida[0] = Entrada5[2];
            Salida[1] = Entrada5[3];
            Salida[2] = Entrada5[4];
            Salida[3] = PrimerValor;
            Salida[4] = SegundoValor;

            return Salida;
        }
        char[] XOR(char[] Value1, char[] Value2) //Función lógica XoR
        {
            char[] resultado = new char[Value1.Length];
            for (int i = 0; i < Value1.Length; i++)
            {
                if (Value1[i] == Value2[i])
                {
                    resultado[i] = '0';
                }
                else
                {
                    resultado[i] = '1';
                }
            }
            return resultado;
        }
        void DividirByte(ref char[] Primero, ref char[] Segundo, char[] Original) //Divide el byte en 2 bytes iguales
        {
            int Limite = Original.Length / 2;
            for (int i = 0; i < (Limite); i++)
            {
                Primero[i] = Original[i];
                Segundo[i] = Original[i + Limite];
            }
        }

        //Definicion algoritmo de cifrado
        void DefinirLlaves(string Llave, ref char[] k1, ref char[] k2)
        {
            char[] PrimeraParte = new char[5];//Almacena la primera parte de permutacion10
            char[] SegundaParte = new char[5];//Almacena la Segunda parte de permutacion10
            char[] P10 = Permutacion10(Llave); //Almacena el valor resultado de la primera permutacion de 10
            for (int i = 0; i < 5; i++)//Divide en dos el resultado de permutacion de 10
            {
                PrimeraParte[i] = P10[i];
                SegundaParte[i] = P10[i + 5];
            } //Dividir el vector P10 en dos partes
              //Haciendo leftshift de los 10 bits
            PrimeraParte = LeftShift1(PrimeraParte);
            SegundaParte = LeftShift1(SegundaParte);
            P10 = PrimeraParte.Concat(SegundaParte).ToArray(); //Unir la primera y segunda parte
            k1 = PermutarYseleccionar(P10); //Se genera el valor de k1

            //Hacer leftshift 2 sobre primera y segunda parte para generar la k2
            PrimeraParte = LeftShift2(PrimeraParte);
            SegundaParte = LeftShift2(SegundaParte);
            //Realizar un permutar y seleccionar como se hizo para k1
            P10 = PrimeraParte.Concat(SegundaParte).ToArray(); //Unir la primera y segunda parte
            k2 = PermutarYseleccionar(P10);

        }
        void IndicesSboxes(ref int Fila, ref int Columna, char[] Origen)
        {
            string fila = "";
            string columna = "";
            fila = Origen[0].ToString() + Origen[3].ToString(); //agarrar filas
            columna += Origen[1].ToString() + Origen[2].ToString(); //agarrar columnas
            Fila = Convert.ToInt32(fila, 2);
            Columna = Convert.ToInt32(columna, 2);

        }
        char[] Generar8Bits(byte caracter) //Hace que todos los bytes sean representados por 8 caracteres
        {
            string ConvertirAbinario = Convert.ToString(caracter, 2); //Convierte el byte a un número binario
            ConvertirAbinario = ConvertirAbinario.PadLeft(8, '0');
            return (ConvertirAbinario.ToCharArray());
        }
        void CifrarCaracteres(byte caracter, char[] k1, char[] k2, ref byte salida) //Método general de cifrado
        {
            char[] Caracter8Bits = Generar8Bits(caracter); //Obteniendo siempre 8 bits independientemente del byte
            char[] Auxiliar = new char[8]; //Vector auxiliar que ayudará a no perder datos en la realizacion del algoritmo
            char[] PrimeraParte = new char[4];
            char[] SegundaParte = new char[4];
            char[] SboxA = new char[4];//Vector auxiliar que ayudará a no perder datos en la realizacion del algoritmo
            char[] SboxB = new char[4];//Vector auxiliar que ayudará a no perder datos en la realizacion del algoritmo               
            char[] ResultadoSbox = new char[4]; //Tiene el resultado de las búsquedas de sbox
            char[] auxiliarB = new char[4]; //Uso variable

            int Fila = 0, Columna = 0; //Variables que se utilizaran para almacenar el indice a buscar en las sboxes
            Caracter8Bits = PermutacionInicial(Caracter8Bits);
            DividirByte(ref PrimeraParte, ref SegundaParte, Caracter8Bits);//Separar la permutacion inicial en dos porque se utilizará la parte dos al hacer el xor

            Auxiliar = ExpandirYPermutar(SegundaParte);//Expandir y permutar la segunda parte
            Auxiliar = XOR(Auxiliar, k1);
            DividirByte(ref SboxA, ref SboxB, Auxiliar); //Dividir el resultado del xor para buscar en las sboxes
            IndicesSboxes(ref Fila, ref Columna, SboxA); //Retorna el indice que se tiene que buscar en las sboxes
            SboxA = Sbox1[Fila, Columna].ToCharArray(); //almacena el resultado de la busqueda del primer bloque de 4 en el sbox1                
            Fila = 0; Columna = 0; //Limpiar los valores de los indices para evitar errores
            IndicesSboxes(ref Fila, ref Columna, SboxB);
            SboxB = Sbox2[Fila, Columna].ToCharArray();
            ResultadoSbox = SboxA.Concat(SboxB).ToArray();
            ResultadoSbox = Permutacion4(ResultadoSbox);
            ResultadoSbox = XOR(ResultadoSbox, PrimeraParte);
            Auxiliar = SegundaParte.Concat(ResultadoSbox).ToArray(); //Se une el resultado de xor con la segunda parte de la permutacion inicial se hace el swap de una vez
                                                                     //EL PROCESO DE ARRIBA SE REPITE 2 VECES PERO AHORA UTILIZANDO LA LLAVE K2

            DividirByte(ref PrimeraParte, ref auxiliarB, Auxiliar); //primera parte tiene la primera parte del auxiliar, ResultadoSbox se recicló para utilizarla como la segunda parte de auxiliar que se necesitara en un futuro
            Auxiliar = ExpandirYPermutar(auxiliarB); //Expande y permuta la segunda parte del resultado del swap
            Auxiliar = XOR(Auxiliar, k2);
            char[] First = new char[4];
            char[] Last = new char[4];
            DividirByte(ref First, ref Last, Auxiliar);
            Fila = 0; Columna = 0; //Limpiar los valores de los indices para evitar errores
            IndicesSboxes(ref Fila, ref Columna, First);
            First = Sbox1[Fila, Columna].ToCharArray(); //Buscar valor en la sbox2 y asignarlo
            Fila = 0; Columna = 0; //Limpiar los valores de los indices para evitar errores
            IndicesSboxes(ref Fila, ref Columna, Last);
            Last = Sbox2[Fila, Columna].ToCharArray();
            ResultadoSbox = First.Concat(Last).ToArray();
            ResultadoSbox = Permutacion4(ResultadoSbox);
            ResultadoSbox = XOR(ResultadoSbox, PrimeraParte);
            Auxiliar = ResultadoSbox.Concat(auxiliarB).ToArray(); //Se une con la segunda parte resultado del swap
            Auxiliar = PermutacionInversa(Auxiliar); //RESULTADO FINAL, ES LO QUE HAY QUE ESCRIBIR EN EL ARCHIVO  
            string ByteAEscribir = "";
            for (int i = 0; i < 8; i++)
            {
                ByteAEscribir += Auxiliar[i];
            }
            int TransformarAbinario = Convert.ToInt32(ByteAEscribir, 2);
            salida = Convert.ToByte(TransformarAbinario); //Los bytes que se tienen que escribir

        }

        //Metodos para lectura y escritura de archivos
        void LeerArchivo(Queue<byte> TextoAleer, string rutaOrigen) //LEER TEXTO UTILIZANDO UN BUFFER Y TODO LEIDO EN BYTES
        {
            const int bufferLength = 1024;
            var buffer = new byte[bufferLength];
            using (var file = new FileStream(rutaOrigen, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            TextoAleer.Enqueue(item);
                        }
                    }

                }

            }


        }
        void EscribirArchivo(Queue<byte> Texto, string ruta) //YA //Escribe cada uno de los elementos existentes en la cola
        {
            using (var file = new FileStream(ruta, FileMode.Create))
            {
                using (var writer = new BinaryWriter(file))
                {
                    while (Texto.Count > 0)
                    {
                        writer.Write(Texto.Dequeue());
                    }
                }
            }
        }
    }
}
