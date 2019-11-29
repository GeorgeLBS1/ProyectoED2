using System;
using System.Collections.Generic;
using System.Text;

namespace AlgoritmosEDII
{
    public struct ClaseNodo
    {
        public string caracter;
        public int frecuencia;
        public string Char_padre; //almacena el nombre del nodo padre
        public string Char_hijoIzquierdo; //Almacenan los nombres de los nodos hijos
        public string Char_hijoDerecho; //Almacenan los nombres de los nodos hijos
        public string camino; //almacena el camino para llegar al nodo
        public string codigo; //almacena el codigo prefijo del nodo (unicamente disponible en las hojas)
    }
}
