using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgoritmosEDII
{
    public class Espiral
    {
        Queue<byte> cola = new Queue<byte>(); //almacenara el texto de entrada
        Queue<byte> TextoImprimir = new Queue<byte>(); //almacena el texto de salida
        int m, residuo; //m es el tamaño de la matriz y residuo son los sobrantes

        //Metodos principales
        public void Encriptar(string RutaArchivoEntrada, int n, int DireccionRecorrido) //Metodo para encriptar el texto
        {
            LeerArchivo(cola, RutaArchivoEntrada);
            DefinirTamanoMatriz(cola, n);
            CrearMatriz(n, m, cola, DireccionRecorrido);
            FileInfo fileInfo = new FileInfo(RutaArchivoEntrada);
            string rutaEncriptada = fileInfo.Name.Replace(".txt", ".cif");
            EscribirArchivo(TextoImprimir, rutaEncriptada);
        }
        public void Desencriptar(string RutaArchivoEntrada, int n, int DireccionRecorrido) //Metodo principal para desencriptar
        {

            LeerArchivo(cola, RutaArchivoEntrada);
            DefinirTamanoMatriz(cola, n);
            byte[,] Matriz = new byte[m, n];
            int CantidadDeValores = m * n;
            MatrizDesencripcion(n, m, cola, DireccionRecorrido, Matriz); //armar la matriz
            TxtDesencriptado(Matriz, n, m); //Obtener el texto que posteriormente se escribira en el archivo
            FileInfo fileInfo = new FileInfo(RutaArchivoEntrada);
            string rutaEncriptada = fileInfo.Name.Replace(".cif", ".txt");
            EscribirArchivo(TextoImprimir, rutaEncriptada);
        }

        //Metodos de escritura y lectura
        void EscribirArchivo(Queue<byte> Texto, string ruta) //Escribe cada uno de los elementos existentes en la cola
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
        void LeerArchivo(Queue<byte> TextoAleer, string rutaOrigen) //Lee el texto del archivo por medio de un buffer
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

        //Métodos mixtos (se usan tanto en el cifrado como en el descifrado)
        void DefinirTamanoMatriz(Queue<byte> Texto, int n) //Calcula el valor de m en base al valor de llave n y así evitar que el tamaño de la matriz sea menor al espeerado
        {

            if (Texto.Count % n == 0)
            {
                m = Texto.Count / n;
                residuo = 0;
            }
            else
            {
                m = (Texto.Count / n) + 1;
                residuo = (m * n) - Texto.Count();
                LlenarEspaciosVacios(Texto, residuo); //si la matriz no esta completamente llena, manda a escribir asteriscos
            }


        }
        void LlenarEspaciosVacios(Queue<byte> TextoEntrada, int resto) // en este metodo se rellenan los espacios que quedan libres en la matriz
        {
            for (int i = 0; i < resto; i++)
            {
                TextoEntrada.Enqueue(Convert.ToByte('*')); //Si sobran espacios en la matriz rellenar con *
            }
        }

        //Métodos Cifrado
        void CrearMatriz(int N, int M, Queue<byte> texto, int Recorrido) //Solo sirve para indicar como hacer el recorrido para encriptar
        {
            byte[,] Matriz = new byte[N, M];
            int CantidadDeValores = M * N;
            for (int i = 0; i < N; i++) //Llenar la matriz con la cola de forma normal
            {
                for (int j = 0; j < M; j++)
                {
                    Matriz[i, j] = texto.Dequeue();
                }
            }

            switch (Recorrido)
            {
                case 1:
                    RecorridoHorario(Matriz, N, M, TextoImprimir); //Si el usuario indica 1 recorrer horario
                    break;
                case 2:
                    RecorridoAntihorario(Matriz, N, M, TextoImprimir); //Si el usuario indica 2 Recorrer antihorario
                    break;
                default:
                    break;
            }

        }
        void RecorridoHorario(byte[,] Matriz, int N, int M, Queue<byte> TextoImprimir) //Recorre la matriz en forma horario y almacena el recorrido en una cola que posteriormente se escribira en un archivo
        {

            int a, b, c, i, j;
            a = 0; //variables para hacer los recorridos
            b = M; //ancho
            c = N;//variables para hacer los recorridos verticales
            j = 0; //variables para hacer los recorridos*/
            while (TextoImprimir.Count < N * M)
            {
                for (i = a; i < c; i++) //Llenado de Columnas Izquierdas
                {
                    TextoImprimir.Enqueue(Matriz[i, j]);
                }
                for (j = a + 1; j < b; j++) //Llenado de filas inferiores
                {
                    TextoImprimir.Enqueue(Matriz[i - 1, j]);
                }
                for (i = c - 1; i > a; i--) //Llenado de columnas derechas
                {
                    TextoImprimir.Enqueue(Matriz[i - 1, j - 1]);
                }

                for (j = b - 1; j > a + 1; j--) //Llenado filas inferiores
                {
                    TextoImprimir.Enqueue(Matriz[i, j - 1]);
                }
                a++;
                b--;
                c--;
            }



        }
        void RecorridoAntihorario(byte[,] Matriz, int N, int M, Queue<byte> TextoImprimir)//Recorre la matriz en forma antihorario y almacena el recorrido en una cola que posteriormente se escribira en un archivo
        {

            int a, b, c, i, j;
            a = 0; //variables para hacer los recorridos
            b = M; //ancho
            c = N;//variables para hacer los recorridos verticales
            i = 0; //variables para hacer los recorridos*/
            while (TextoImprimir.Count < N * M)
            {
                for (j = a; j < b; j++) //Llenado de filas superiores
                {
                    TextoImprimir.Enqueue(Matriz[i, j]);
                }
                for (i = a + 1; i < c; i++) //Llenado de columnas derechas
                {
                    TextoImprimir.Enqueue(Matriz[i, j - 1]);
                }
                for (j = b - 1; j > a; j--) //Llenado filas inferiores
                {
                    TextoImprimir.Enqueue(Matriz[i - 1, j - 1]);
                }

                for (i = c - 1; i > a + 1; i--) //Llenado de filas superiores
                {
                    TextoImprimir.Enqueue(Matriz[i - 1, j]);
                }
                a++;
                b--;
                c--;
            }

        }

        //Métodos Decodificacion
        void MatrizDesencripcion(int N, int M, Queue<byte> texto, int Recorrido, byte[,] Matriz) //Metodo para elegir el tipo de recorrido que se usará para el algoritmo
        {

            switch (Recorrido)
            {
                case 2:
                    EscrituraHorario(Matriz, N, M, texto); //Se escribe la matriz contraria a la forma en la que se encripto
                    break;
                case 1:
                    EscrituraAntihorario(Matriz, N, M, texto);
                    break;
                default:
                    break;
            }

        }
        void EscrituraHorario(byte[,] Matriz, int N, int M, Queue<byte> Texto) //llenado de la matriz de forma horario
        {
            int a, b, c, i, j;
            a = 0; //variables para hacer los recorridos
            b = N; //ancho
            c = M;//variables para hacer los recorridos verticales
            j = 0; //variables para hacer los recorridos*/
            while (Texto.Count > 0)
            {
                for (i = a; i < c; i++) //Llenado de Columnas Izquierdas
                {
                    Matriz[i, j] = Texto.Dequeue();
                }
                for (j = a + 1; j < b; j++) //Llenado de filas inferiores
                {
                    Matriz[i - 1, j] = Texto.Dequeue();
                }
                for (i = c - 1; i > a; i--) //Llenado de columnas derechas
                {
                    Matriz[i - 1, j - 1] = Texto.Dequeue();
                }

                for (j = b - 1; j > a + 1; j--) //Llenado filas inferiores
                {
                    Matriz[i, j - 1] = Texto.Dequeue();
                }
                a++;
                b--;
                c--;
            }
        }
        void EscrituraAntihorario(byte[,] Matriz, int N, int M, Queue<byte> Texto) //Llenado de la matriz de forma antihorario
        {
            int a, b, c, i, j;
            a = 0; //variables para hacer los recorridos
            b = N; //ancho
            c = M;//variables para hacer los recorridos verticales
            i = 0; //variables para hacer los recorridos*/
            while (Texto.Count > 0)
            {
                for (j = a; j < b; j++) //Llenado de filas superiores
                {
                    Matriz[i, j] = Texto.Dequeue();
                }
                for (i = a + 1; i < c; i++) //Llenado de columnas derechas
                {
                    Matriz[i, j - 1] = Texto.Dequeue();
                }
                for (j = b - 1; j > a; j--) //Llenado filas inferiores
                {
                    Matriz[i - 1, j - 1] = Texto.Dequeue();
                }

                for (i = c - 1; i > a + 1; i--) //Llenado de filas superiores
                {
                    Matriz[i - 1, j] = Texto.Dequeue();
                }
                a++;
                b--;
                c--;
            }
        }
        void TxtDesencriptado(byte[,] Matriz, int N, int M) //Este metodo descifra el mensaje recorriendo de forma vertical la matriz
        {
            for (int j = 0; j < N; j++)
            {
                for (int i = 0; i < M; i++) //Recorre la matriz de forma vertical
                {
                    if (Matriz[i, j] != 42) //Elimina el separador "*"
                    {
                        TextoImprimir.Enqueue(Matriz[i, j]);
                    }

                }
            }
        }
    }
}
