using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ProyectoED2.Utilities
{
    public class GenerarClavesSeguras
    {
        public int GenerarLlave(int miLlave, int LlaveEmisor)
        {
            int LlavePublica = Llave(LlaveEmisor);
            int LlaveComun = DiffieHelman(miLlave, LlavePublica);
            return LlaveComun;
        }
        int Llave(int LlaveInsegura)
        {
            BigInteger p = 17;
            BigInteger g = 989;
            BigInteger Llave = new BigInteger(LlaveInsegura);
            int Resultado = (int)BigInteger.ModPow(g, Llave, p);
            return Resultado;
        }

        int DiffieHelman(int LlaveInseguraPropia, int LlavePublica)
        {
            BigInteger P = 17;
            BigInteger LlaveInsegura = new BigInteger(LlaveInseguraPropia);
            BigInteger LlaveP = new BigInteger(LlavePublica);
            int K = (int)BigInteger.ModPow(LlaveP, LlaveInsegura, P);
            return K;

        }
    }
}
