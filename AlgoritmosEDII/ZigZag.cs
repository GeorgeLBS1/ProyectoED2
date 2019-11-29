using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlgoritmosEDII
{
    public class ZigZag
    {
        public void Cifrar(string Ruta, int ClaveCifrado)
        {
            //Cola que recibe los bytes del texto que se desea cifrar
            Queue<byte> TextoEntrada = new Queue<byte>();
            //Variables auxiliares para el lector binario
            const int bufferLength = 1024;
            var buffer = new byte[bufferLength];
            //Lector binario
            using (var file = new FileStream(Ruta, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            TextoEntrada.Enqueue(item);
                        }
                    }
                }
            }
            //Calcular la cantidad de caracteres en una de las olas (ondas) del zig-zag
            int CantidadCaracteresOla = (ClaveCifrado * 2) - 2;
            //Agregar la cantidad de caracteres necesarios para completar una ola
            int LlenarOla = (CantidadCaracteresOla - (TextoEntrada.Count % CantidadCaracteresOla) + 1);
            for (int i = 0; i < LlenarOla; i++)
            {
                TextoEntrada.Enqueue(32);
            }
            //Convertir la cola TextoEntrada a un arreglo de bytes
            int TamañoColaEntrada = TextoEntrada.Count;
            byte[] CifraBytes = new byte[TamañoColaEntrada];
            for (int i = 0; i < TamañoColaEntrada; i++)
            {
                CifraBytes[i] = TextoEntrada.Dequeue();
            }
            //Arreglo en donde se almacenan los caracteres, según el nivel en que se encuentran 
            Queue<byte>[] Colabytes = new Queue<byte>[ClaveCifrado];
            //Inicializar colas del arreglo
            for (int i = 0; i < Colabytes.Length; i++)
            {
                Colabytes[i] = new Queue<byte>();
            }
            //Variable que indica en que nivel del zig-zag se encuentra un caracter
            int NivelActual = 0;
            //Variable que indica en que dirección del zig-zag se está recorrienco el arreglo de caracteres (hacia la parte superior o inferior de una ola)
            int CambioNivel = 1;
            byte Aux;
            //Recorrer el arreglo de caracteres y llenar las colas del arreglo Colabytes
            for (int i = 0; i < CifraBytes.Length; i++)
            {
                Aux = CifraBytes[i];
                Colabytes[NivelActual].Enqueue(Aux);
                if (NivelActual == 0)
                {
                    CambioNivel = 1;
                }
                else if (NivelActual == ClaveCifrado - 1)
                {
                    CambioNivel = -1;
                }
                NivelActual += CambioNivel;
            }
            //Cola que recibe el textocifrado
            Queue<byte> TextoCifrado = new Queue<byte>();
            foreach (var Cola in Colabytes)
            {
                foreach (var UnByte in Cola)
                {
                    TextoCifrado.Enqueue(UnByte);
                }
            }
            //Escritura del archivo .cif
            FileInfo CambioExtension = new FileInfo(Ruta);
            string Salida = CambioExtension.Name.Replace(".txt", ".cif");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Salida);
            using (var file2 = new FileStream(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(file2))
                {
                    while (TextoCifrado.Count > 0)
                    {
                        writer.Write(TextoCifrado.Dequeue());
                    }
                }
            }
        }
        public void Descifrar(string Ruta, int ClaveDescifrado)
        {
            //Cola que recibe los bytes del texto que se desea cifrar
            Queue<byte> TextoEntrada = new Queue<byte>();
            //Variables auxiliares para el lector binario
            const int bufferLength = 1024;
            var buffer = new byte[bufferLength];
            //Lector binario
            using (var file = new FileStream(Ruta, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            TextoEntrada.Enqueue(item);
                        }
                    }
                }
            }
            //Convertir la cola con los bytes del texto en un arreglo de bytes
            //Convertir la cola TextoEntrada a un arreglo de bytes
            int TamañoColaEntrada = TextoEntrada.Count;
            byte[] CifraBytes = new byte[TamañoColaEntrada];
            for (int i = 0; i < TamañoColaEntrada; i++)
            {
                CifraBytes[i] = TextoEntrada.Dequeue();
            }
            //Arreglo en donde se almacenan los caracteres, según el nivel en que se encuentran 
            Queue<byte>[] Colabytes = new Queue<byte>[ClaveDescifrado];
            //Inicializar colas del arreglo
            for (int i = 0; i < Colabytes.Length; i++)
            {
                Colabytes[i] = new Queue<byte>();
            }
            //arreglo con el que se cuenta la cantidad de caracteres que se encuentran en el mismo nivel
            int[] LongitudNiveles = new int[ClaveDescifrado];
            //Variable que indica en que nivel del zig-zag se encuentra un caracter
            int NivelActual = 0;
            //Variable que indica en que dirección del zig-zag se está recorrienco el arreglo de caracteres (hacia la parte superior o inferior de una ola)
            int CambioNivel = 1;
            //Contar Caracteres del nivel
            for (int i = 0; i < CifraBytes.Length; i++)
            {
                LongitudNiveles[NivelActual]++;
                if (NivelActual == 0)
                {
                    CambioNivel = 1;
                }
                else if (NivelActual == ClaveDescifrado - 1)
                {
                    CambioNivel = -1;
                }
                NivelActual += CambioNivel;
            }
            //Recopilar los caracteres de cada nivel
            int CaracterActual = 0;
            //Recopilar los caracteres de cada Nivel
            for (int i = 0; i < ClaveDescifrado; i++)
            {
                for (int j = 0; j < LongitudNiveles[i]; j++)
                {
                    Colabytes[i].Enqueue(CifraBytes[CaracterActual]);
                    CaracterActual++;
                }
            }
            NivelActual = 0;
            CambioNivel = 1;
            //Cola que almacenará los bytes del texto en proceso de descifrado
            Queue<byte> TextoDescifrado = new Queue<byte>();
            for (int i = 0; i < CifraBytes.Length; i++)
            {
                TextoDescifrado.Enqueue(Colabytes[NivelActual].Dequeue());
                if (NivelActual == 0)
                {
                    CambioNivel = 1;
                }
                else if (NivelActual == ClaveDescifrado - 1)
                {
                    CambioNivel = -1;
                }
                NivelActual += CambioNivel;
            }
            //Escritura del archivo .cif
            FileInfo CambioExtension = new FileInfo(Ruta);
            string Salida = CambioExtension.Name.Replace(".cif", ".txt");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Salida);
            using (var file2 = new FileStream(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(file2))
                {
                    while (TextoDescifrado.Count > 0)
                    {
                        writer.Write(TextoDescifrado.Dequeue());
                    }
                }
            }
        }
    }
}
