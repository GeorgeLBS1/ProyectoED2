using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoED2.Utilities
{
    public class HelperArchivos
    {
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
        public void EscribirArchivo(Queue<byte> Texto, string ruta)//Escribe cada uno de los elementos existentes en la cola
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
