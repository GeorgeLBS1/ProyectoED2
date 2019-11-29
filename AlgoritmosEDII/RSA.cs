using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace AlgoritmosEDII
{
    public class RSA
    {
        int p = 0, q = 0, n = 0, PhideN = 0, e = 0, D; //Variables que se usaran para generar las llaves.
        Queue<byte> TextoEntrada = new Queue<byte>(); //Se almacena todo el texto del archivo en una cola para evitar ovverflow
        Queue<byte> TextoCifrado = new Queue<byte>(); //Texto resultado del cifrado, se escribirá en el archivo

        //MÉTODOS PRINCIPALES
        public void GenerarLLaves(int P, int Q)//asigna los valores de p y q además de crear n y phi de N
        {
            p = P;
            q = Q;
            n = (p * q); //calcular n
            PhideN = (p - 1) * (q - 1); //Calcular phi de n
        CalculoE:
            e = EncontrarE(PhideN, n); //encontrar e
            D = InversoModular(e, PhideN); //Se encuentra D que es el inverso modular de e y PhideN
            if (D == e) //En dado caso el numero e y el d sean iguales (SÍ pasa [lo experimenté un par de veces]) volver a buscar un e y volver a calcular d
            {
                goto CalculoE;
            }
            var LlavePublica = Tuple.Create(n, e); //Se guardan los valores de la llave publica
            var LlavePrivada = Tuple.Create(n, D); //Se guardan los valores de la llave privada
            var pathA = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "public.key");
            var pathB = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "private.key");
            FileWrite(pathA, LlavePublica);
            FileWrite(pathB, LlavePrivada);
        }
        public void Cifrar(string ArchivoKey, string ArchivoTexto) //Cifrar el texto
        {
            Tuple<int, int> llave = LeerLlaves(ArchivoKey);
            LeerArchivo(TextoEntrada, ArchivoTexto);
            int E = llave.Item1;
            int N = llave.Item2;
            while (TextoEntrada.Count > 0)
            {
                //Se guardará el valor que posteriormente se escribirá
                BigInteger basee = new BigInteger(TextoEntrada.Dequeue());
                BigInteger Exponente = new BigInteger(N);
                BigInteger Modular = new BigInteger(E);
                int Cifrado = (int)(BigInteger.ModPow(basee, Exponente, Modular)); //Esta función permite sacar el módulo de un numero muy grande elevado a cierta potencia
                //BigInteger parte1 = new BigInteger((Math.Pow(Convert.ToInt64(TextoEntrada.Dequeue()), N)) % E);
                if (Cifrado < 256) //En dado caso el mod fuese mayor se hace la conversion a escribir el byte en 3 bytes así se obtiene una capacidad total aprox de 16 millones
                {
                    TextoCifrado.Enqueue(0);
                    TextoCifrado.Enqueue(0);
                    TextoCifrado.Enqueue(Convert.ToByte(Cifrado));
                }
                if (Cifrado > 255 && Cifrado < 65026)
                {
                    TextoCifrado.Enqueue(0);
                    int cociente = Cifrado / 255;
                    TextoCifrado.Enqueue(Convert.ToByte(cociente));
                    int residuo = Cifrado % 255;
                    TextoCifrado.Enqueue(Convert.ToByte(residuo));
                }
                if (Cifrado > 65026)
                {
                    int cociente = Cifrado / 65025;
                    TextoCifrado.Enqueue(Convert.ToByte(cociente));
                    cociente = Cifrado % 65025;
                    TextoCifrado.Enqueue(Convert.ToByte((cociente / 255)));
                    int residuo = cociente % 255;
                    TextoCifrado.Enqueue(Convert.ToByte(residuo));

                }
            } //Cifrar los caracteres y guardarlos en la cola
            //INTERCAMBIO DE NOMBRE AL ARCHIVO DE TEXTO
            FileInfo fileInfo = new FileInfo(ArchivoTexto);
            string rutaEncriptada = fileInfo.Name.Replace(".txt", ".cif");
        }
        public void Descifrar(string ArchivoKey, string ArchivoTexto) //Descifrar el texto
        {
            Queue<int> BytesOriginales = new Queue<int>(); //Contendrá los 3 bytes ya depurados en un único valor entero
            Tuple<int, int> llave = LeerLlaves(ArchivoKey);
            LeerArchivo(TextoEntrada, ArchivoTexto);
            int E = llave.Item1;
            int N = llave.Item2;

            DepurarDataCifrada(BytesOriginales, TextoEntrada);
            while (BytesOriginales.Count > 0)
            {
                //Se guardará el valor que posteriormente se escribirá
                BigInteger basee = new BigInteger(BytesOriginales.Dequeue());
                BigInteger Exponente = new BigInteger(N);
                BigInteger Modular = new BigInteger(E);
                int DesCifrado = (int)(BigInteger.ModPow(basee, Exponente, Modular)); //Esta función permite sacar el módulo de un numero muy grande elevado a cierta potencia
                if (DesCifrado > 255) //Esto solo pasaría si se ingresa una llave incorrecta|| para que se pueda devolver un archivo
                {
                    DesCifrado %= 40;
                }
                TextoCifrado.Enqueue(Convert.ToByte(DesCifrado)); //Guardar el caracter descifrado
            }
            //INTERCAMBIO DE NOMBRE AL ARCHIVO DE TEXTO
            FileInfo fileInfo = new FileInfo(ArchivoTexto);
            string rutaEncriptada = fileInfo.Name.Replace(".cif", ".txt");
            EscribirArchivo(TextoCifrado, rutaEncriptada);
        }
        //VERIFICADORES LLAVE
        public bool EsPrimo(int Numero) //Verificar si el numero es o no primo
        {
            int Divisores = 0; //Numero de divisores
            for (int i = 1; i <= Numero; i++)
            {
                if (Numero % i == 0)
                {
                    Divisores++;
                }
            }
            if (Divisores == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool NumerosDiferentes(int a, int b) //Verifica que ambos numeros de entrada para generar las llaves no estén repetidos
        {
            if (a != b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Mayores_A_255(int a, int b) //Se verifica que la multiplicacion de las llaves sea mayor a 255 para no tener problemas con los bytes
        {
            if ((a * b) >= 255)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //GENERACIÓN DE LLAVES
        int MCD(int NumeroMayor, int NumeroMenor) //Calcular MCD de dos números
        {
            int Resultado = 0;
            do //Algoritmo de euclides para calcular el MCD de dos números
            {
                Resultado = NumeroMenor;
                NumeroMenor = NumeroMayor % NumeroMenor;
                NumeroMayor = Resultado;
            } while (NumeroMenor != 0);
            return Resultado;
        }
        int EncontrarE(int Phi, int N) //Encuentra el valor de "e" que es cualquier coprimo de N y phi de N
        {
            List<int> Coprimos = new List<int>();
            for (int i = 2; i < Phi; i++) //Calcular todos los coprimos de Phi de N y N
            {
                if ((MCD(Phi, i) == 1) && (MCD(N, i) == 1))
                {
                    Coprimos.Add(i);
                }

            }
            Random r = new Random();
            return Coprimos[r.Next(0, Coprimos.Count / 2)]; //Se escoge un coprimo random (pero de la mitad mas pequeña, así evitar numeros inmensos a la hora de calcular D) para asignarlo como e

        }
        int InversoModular(int Entrada, int modulo) //Calcula el inverso multiplicativo modular de dos números en base al método de la tabla visto en laboratorio
        {
            int AuxiliarModulo = modulo, salida = 0, Columna = 1;
            while (Entrada > 0)
            {
                int Entero_SiguienteIteracion = AuxiliarModulo / Entrada, ModuloPrevio = Entrada;
                Entrada = AuxiliarModulo % ModuloPrevio;
                AuxiliarModulo = ModuloPrevio;
                ModuloPrevio = Columna;
                Columna = salida - Entero_SiguienteIteracion * ModuloPrevio;
                salida = ModuloPrevio;

            }
            salida %= modulo;
            if (salida < 0) salida = (salida + modulo) % modulo;
            return salida;
        }
        void FileWrite(string name, Tuple<int, int> tuple) //Escribir las llaves en archivo de texto
        {
            StreamWriter writer = new StreamWriter(name);
            writer.Write(tuple.Item1 + "," + tuple.Item2);
            writer.Close();
        }

        //CIFRADO Y DESCIFRADO (la mayoría son métodos relacionados a la lectura y escritura del archivo)
        Tuple<int, int> LeerLlaves(string path) //Leer las llaves y pasarlas al formato de Tuple para facilidad del manejo de datos
        {
            StreamReader reader = new StreamReader(path);
            string texto = reader.ReadLine();
            string[] split = texto.Split(',');
            int E = Convert.ToInt32(split[0]);
            int N = Convert.ToInt32(split[1]);
            var llave = Tuple.Create(E, N);
            reader.Close();
            return llave;
        }
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
        void EscribirArchivo(Queue<byte> Texto, string ruta)//Escribe cada uno de los elementos existentes en la cola
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
        void DepurarDataCifrada(Queue<int> Salida, Queue<byte> Texto3Bytes)//Convertir los 3 bytes en uno solo
        {
            int Encolar; //En este int irá el número resultado de la depuración de la lista anterior
            int Casilla1, Casilla2, Casilla3;
            while (Texto3Bytes.Count > 0)
            {
                Casilla1 = (int)Texto3Bytes.Dequeue();
                Casilla2 = (int)Texto3Bytes.Dequeue();
                Casilla3 = (int)Texto3Bytes.Dequeue();
                Encolar = (Casilla1 * 65025) + (Casilla2 * 255) + Casilla3;
                Salida.Enqueue(Encolar);
            }
        }
    }
}
