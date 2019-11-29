using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlgoritmosEDII
{
    public class Huffman
    {
        int total_de_caracteres; //Variable que cuenta el total de caracteres del archivo de texto
        public void Comprimir(string ruta) //METODO BASE PARA INICIAR COMPRESION
        {
            string cadenaPrefijo = ""; //almacena todo el texto prefijo
            string archivo; //almacena todo el texto leido del archivo
            List<ClaseNodo> ListaNodosArbol = new List<ClaseNodo>(); //Lista que contiene a todos los nodos que se irán armando
            List<LetraNodo> Diccionario = new List<LetraNodo>(); //diccionario con los prefijos

            //LECTURA DEL ARCHIVO DE TEXTO
            StreamReader reader = new StreamReader(ruta, Encoding.UTF8);
            char[] texto; //Almacenará todos los caracteres leidos del archivo txt
            archivo = reader.ReadToEnd();
            reader.Close();
            FileInfo fileInfo = new FileInfo(ruta);
            string OutRuta = fileInfo.Name.Replace(".txt", ".huff"); //almacena la ruta de salida del archivo .huff
            texto = archivo.ToCharArray(); //Convierte todo el texto en un array de chars
            total_de_caracteres = texto.Length; //Almacena la cantidad total de caracteres
            contarCaracteres(texto, ListaNodosArbol); //contar los caracteres y sus frecuencia
            completarNodos(ListaNodosArbol);
            BubbleSort(ListaNodosArbol);
            JuntarNodos(ListaNodosArbol); //crea el arbol agrupando los nodos de mayores frecuencias.

            Diccionario = CrearPrefijos(ListaNodosArbol); //Guarda los codigos prefijos en un diccionario

            //Vectores para sustituir por los codigos prefijo a las letras
            char[] VectorChar = archivo.ToCharArray();
            string[] VectorString = new string[VectorChar.Length];

            //Convirtiendo el vector de chars en uno de strings para poder escribir los codigos prefijo
            for (int i = 0; i < VectorChar.Length; i++)
            {
                VectorString[i] = Convert.ToString(VectorChar[i]);
            }

            //Sustituyendo los caracteres por los codigos prefijo
            for (int i = 0; i < VectorString.Length; i++)
            {
                foreach (LetraNodo Caracter in Diccionario)
                {
                    if (VectorString[i] == Caracter.caracter)
                    {
                        VectorString[i] = Caracter.llave;
                        cadenaPrefijo += VectorString[i];
                    }
                }
            }
            int ContadorDeAgregados = 0;
            //----------------------IMPORTANTE----------------------------
            //añade los 0 necesarios para formar los bloques de 8 bits
            while (!(cadenaPrefijo.Length % 8 == 0))
            {
                cadenaPrefijo += "0";
                ContadorDeAgregados++;
            }
            //creando bloques de 8 bits

            List<int> Bloquesde8 = new List<int>();
            List<byte> Arrbytes = new List<byte>();
            int Guard;
            //Variables auxiliares para la escritura a decimal de cadenaPrefijo
            char[] Grupo8Bits = new char[8];
            //Este ciclo recorre el string del texto y agrupa en bloques de 8

            for (int i = 0; i < cadenaPrefijo.Length; i = i + 8)
            {
                string respuesta = cadenaPrefijo.Substring(i, 8);
                //Variables auxiliares para la escritura a decimal de cadenaPrefijo
                Grupo8Bits = respuesta.ToCharArray();
                string ValorBintoDec = "";
                int IngresaraArryBytes;
                if (Grupo8Bits[0] == '1')
                {
                    Guard = Convert.ToInt32(respuesta, 2);
                    Arrbytes.Add(Convert.ToByte(Guard));
                    Bloquesde8.Add(Guard);
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (Grupo8Bits[j] == '0')
                        {
                            Arrbytes.Add(Convert.ToByte(48));
                            Bloquesde8.Add(48);
                        }
                        else
                        {
                            for (int k = j; k < 8; k++)
                            {
                                ValorBintoDec += Grupo8Bits[k].ToString();
                            }
                            IngresaraArryBytes = Convert.ToInt32(ValorBintoDec, 2);
                            Arrbytes.Add(Convert.ToByte(IngresaraArryBytes));
                            Bloquesde8.Add(IngresaraArryBytes);
                            break;
                        }
                    }

                }


            }

            //Escribir archivo de .huff
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        OutRuta);
            StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);

            foreach (var item in Diccionario)
            {
                writer.Write(item.caracter + "}" + item.frecuencia + "|");
            }
            string SeparadorCodigo = "-_ ";
            writer.Write(SeparadorCodigo);
            writer.Close();
            AppendAllBytes(path, Arrbytes.ToArray());
        }
        public void Descomprimir(string ruta) //Método BASE para realizar la descompresión
        {
            //Recepción y acceso al archivo a descomprimir
            using (FileStream ArchivoComprimido = new FileStream(ruta, FileMode.Open, FileAccess.Read))
            {
                //Creación de variables
                byte[] ContenidoBytes = new byte[ArchivoComprimido.Length];//Vector que va a recibir los bytes del archivo
                int BytesporLeer = (int)ArchivoComprimido.Length;//Longitud del archivo
                int BytesLeidos = 0;//Cantidad de carcateres que ya se han leido
                while (BytesporLeer > 0)
                {
                    int Contador = ArchivoComprimido.Read(ContenidoBytes, BytesLeidos, BytesporLeer);
                    if (Contador == 0)
                    {
                        break;
                    }
                    BytesLeidos += Contador;
                    BytesporLeer -= Contador;
                }
                BytesporLeer = ContenidoBytes.Length;
                //Conversión de bytes a caracteres
                string BytesACaracteres = Encoding.UTF8.GetString(ContenidoBytes);
                //Separar la tabla de frecuencias del código comprimido
                char[] Separador = new char[3];
                Separador[0] = '-';
                Separador[1] = '_';
                Separador[2] = ' ';
                string[] SepararContenido = BytesACaracteres.Split(Separador);
                //Eliminar Byte Order Marker
                char[] LetrasYFrecuencias = SepararContenido[0].ToCharArray();
                string TablaFrecuencias = "";
                for (int i = 1; i < LetrasYFrecuencias.Length; i++)
                {
                    TablaFrecuencias += LetrasYFrecuencias[i];
                }
                //Vaciar el string para llenarlo con la cadena binaria
                SepararContenido[1] = "";
                //Obtener la cantidad de bytes que ocupa la tabla de letras y frecuencias
                byte[] Muestra = Encoding.UTF8.GetBytes(SepararContenido[0]);
                int Índice = Muestra.Length + 3;
                int LongitudCódigo = ContenidoBytes.Length - Índice;
                byte[] BytesCódigo = new byte[LongitudCódigo];
                //Copiar los mismos bytes que fueron escritos para comprimir el texto original
                for (int i = Índice; i < ContenidoBytes.Length; i++)
                {
                    BytesCódigo[i - Índice] = ContenidoBytes[i];
                }
                //Lista usada para hacer el árbol
                List<ClaseNodo> ListaNodosArbol = new List<ClaseNodo>();
                //string TablaFrecuencias = SepararContenido[0];
                //Separar las letras y sus respectivos valores de los demás miembros de la tabla
                string[] SepararLetras = TablaFrecuencias.Split('|');
                string[] SepararLetrasYFrecuencias;
                int TotalCaracteres = 0;
                for (int i = 0; i < SepararLetras.Length - 1; i++)
                {
                    ClaseNodo MiembroTabla = new ClaseNodo();
                    SepararLetrasYFrecuencias = SepararLetras[i].Split('}');
                    int Frecuencia = Convert.ToInt32(SepararLetrasYFrecuencias[1]);
                    MiembroTabla.caracter = SepararLetrasYFrecuencias[0];
                    MiembroTabla.frecuencia = Frecuencia;
                    MiembroTabla.Char_hijoIzquierdo = "";
                    MiembroTabla.Char_padre = "";
                    MiembroTabla.Char_hijoDerecho = "";
                    MiembroTabla.camino = "";
                    MiembroTabla.codigo = "";
                    ListaNodosArbol.Add(MiembroTabla);
                    TotalCaracteres += Frecuencia;

                }
                completarNodos1(ListaNodosArbol);
                BubbleSort1(ListaNodosArbol);
                JuntarNodos1(ListaNodosArbol); //crea el arbol agrupando los nodos de mayores frecuencias.
                List<LetraNodo> Diccionario = new List<LetraNodo>(); //diccionario con los prefijos
                Diccionario = CrearPrefijos1(ListaNodosArbol);
                //Indicador del tamaño de arreglos auxiliares
                int CantidadBytesCódigo = BytesCódigo.Length;
                //Variables auxiliares para convertir a binario los StringBytesCódigo
                int[] IntBytesCódigo = new int[CantidadBytesCódigo];
                string stringByte;
                for (int i = 0; i < CantidadBytesCódigo; i++)
                {
                    stringByte = BytesCódigo[i].ToString();
                    IntBytesCódigo[i] = Convert.ToInt32(stringByte);
                }
                //Arreglo que recibe como string los bytes del texto
                string[] StringBytesCódigo = new string[CantidadBytesCódigo];
                for (int i = 0; i < CantidadBytesCódigo; i++)
                {
                    StringBytesCódigo[i] = Convert.ToString(IntBytesCódigo[i], 2);
                    if (StringBytesCódigo[i] != "110000")
                    {
                        SepararContenido[1] += StringBytesCódigo[i];
                    }
                    else if (StringBytesCódigo[i] == "110000")
                    {
                        SepararContenido[1] += "0";
                    }
                }
                //Variables para traducir la cadena de 0's y 1's
                char[] TextoBinario = SepararContenido[1].ToCharArray();
                string Traductor = "";
                string Traducción = "";
                for (int i = 0; i < TextoBinario.Length; i++)
                {
                    Traductor += TextoBinario[i];
                    foreach (var item in Diccionario)
                    {
                        if (Traductor == item.llave)
                        {
                            Traducción += item.caracter;
                            Traductor = "";
                        }
                    }
                }

                //Escribir archivo txt
                FileInfo fileInfo = new FileInfo(ruta);
                string OutRuta = fileInfo.Name.Replace(".huff", ".txt");
                StreamWriter EscribirTraducción = new StreamWriter(OutRuta);
                EscribirTraducción.Write(Traducción);
                EscribirTraducción.Close();
            }
        }
        public static void AppendAllBytes(string path, byte[] bytes) //Metodo para escribir el codigo prefijo al final del archivo
        {

            using (var stream = new FileStream(path, FileMode.Append))
            {

                stream.Write(bytes, 0, bytes.Length);
            }
        }
        void contarCaracteres(char[] Texto, List<ClaseNodo> ListaNodosArbol) //Metodo para contar las frecuencias de los caracteres
        {
            foreach (char item in Texto)
            {
                ClaseNodo p = new ClaseNodo();
                if (ListaNodosArbol.Count == 0)
                {
                    p.caracter = item.ToString();
                    p.frecuencia = 1;
                    p.camino = "";
                    p.Char_hijoDerecho = "";
                    p.Char_padre = "";
                    p.Char_hijoIzquierdo = "";
                    p.codigo = "";
                    ListaNodosArbol.Add(p);
                }
                else
                {

                    if (ListaNodosArbol.Exists(x => x.caracter == item.ToString()))
                    {
                        for (int i = 0; i < ListaNodosArbol.Count; i++)
                        {
                            if (ListaNodosArbol[i].caracter == item.ToString())
                            {
                                ClaseNodo a = new ClaseNodo();
                                a.frecuencia = ListaNodosArbol[i].frecuencia + 1;
                                a.caracter = ListaNodosArbol[i].caracter;
                                a.camino = "";
                                a.Char_hijoDerecho = "";
                                a.Char_hijoIzquierdo = "";
                                a.codigo = "";
                                a.Char_padre = "";
                                ListaNodosArbol[i] = a;
                            }

                        }

                    }
                    else
                    {
                        p.caracter = item.ToString();
                        p.frecuencia = 1;
                        ListaNodosArbol.Add(p);
                    }


                }
            }
        }
        void completarNodos(List<ClaseNodo> ListaNodosArbol) //Termina de inicializar el nodo para cada caracter diferente
        {
            for (int i = 0; i < ListaNodosArbol.Count; i++)
            {
                if (ListaNodosArbol[i].Char_hijoDerecho == null)
                {
                    ClaseNodo a = new ClaseNodo();
                    a.caracter = ListaNodosArbol[i].caracter;
                    a.frecuencia = ListaNodosArbol[i].frecuencia;
                    a.camino = "";
                    a.codigo = "";
                    a.Char_padre = "";
                    a.Char_hijoIzquierdo = "";
                    a.Char_hijoDerecho = "";
                    ListaNodosArbol[i] = a;
                }
            }
        }

        //metodo para el arbol
        void JuntarNodos(List<ClaseNodo> ListaNodosArbol) //Metodo para unir los nodos con mayor frecuencia y formar el arbol, este método da origen al arbol
        {
            //Contador unicamente para asignarle nombre a los nuevos nodos
            int NombreNodosNuevos = 0;
            int i;
            //Nodos a usar
            ClaseNodo NodoNuevo;
            ClaseNodo NodoActual;
            ClaseNodo NodoSiguiente;
            bool LlegarAlaRaiz = false; //verificador si el tamaño de la listanodosarbol es 1

            //inicializar el nodo nuevo

            NodoNuevo.caracter = "";
            NodoNuevo.frecuencia = 0;
            NodoNuevo.Char_padre = "";
            NodoNuevo.Char_hijoIzquierdo = "";
            NodoNuevo.Char_hijoDerecho = "";
            NodoNuevo.camino = "";
            NodoNuevo.codigo = "";

            while (!LlegarAlaRaiz)
            {
                //Crear el nodo padre con los dos nodos mayores
                for (i = 0; i < ListaNodosArbol.Count; i++)
                {
                    //solo queda un elemento en la raiz
                    if (i == ListaNodosArbol.Count - 1)
                    {
                        LlegarAlaRaiz = true;
                        break;
                    }
                    else
                    {
                        //obtener el nodo actual recorrido (el mayor)
                        NodoActual = ListaNodosArbol[i];
                        //Verificar que no tenga padre así dar prioridad a los nodos que poseen un caracter
                        if (NodoActual.Char_padre == "")
                        {
                            //obtener el nodo siguiente mas pequeño y juntarlos para crear un arbol pequeño
                            NodoSiguiente = ListaNodosArbol[i + 1];

                            //Crear el nombre del nodo nuevo N + i y crear el nodo
                            NombreNodosNuevos++;
                            NodoNuevo.caracter = "N" + NombreNodosNuevos;
                            NodoNuevo.frecuencia = NodoActual.frecuencia + NodoSiguiente.frecuencia;
                            NodoNuevo.Char_hijoIzquierdo = NodoActual.caracter;
                            NodoNuevo.Char_hijoDerecho = NodoSiguiente.caracter;

                            ListaNodosArbol.Add(NodoNuevo); //agregar el nodo nuevo al arbol

                            //actualizar nodos anteriores                            
                            NodoActual.Char_padre = NodoNuevo.caracter;
                            NodoActual.camino = "0";
                            NodoSiguiente.Char_padre = NodoNuevo.caracter;
                            NodoSiguiente.camino = "1";
                            //Meterlos con sus valores actuales al arbol
                            ListaNodosArbol[i] = NodoActual;
                            ListaNodosArbol[i + 1] = NodoSiguiente;

                            break;
                        }
                    }
                }
                if (!LlegarAlaRaiz)
                {
                    //Ordenar el arbol de nuevo
                    BubbleSort(ListaNodosArbol);
                }
            }

        }

        void BubbleSort(List<ClaseNodo> ArbolHUFF) //Ordenar los nodos del arbol segun la frecuencia
        {
            ClaseNodo xNodo;
            ClaseNodo yNodo;
            int ContadorNodo;
            int ContadorBubble;
            for (ContadorNodo = 0; ContadorNodo < ArbolHUFF.Count - 2; ContadorNodo++)
            {
                xNodo = ArbolHUFF[ContadorNodo];

                for (ContadorBubble = ContadorNodo + 1; ContadorBubble < ArbolHUFF.Count - 1; ContadorBubble++)
                {
                    yNodo = ArbolHUFF[ContadorBubble];

                    if (xNodo.frecuencia > yNodo.frecuencia)
                    {
                        ArbolHUFF[ContadorNodo] = yNodo;
                        ArbolHUFF[ContadorBubble] = xNodo;
                        xNodo = yNodo;
                    }
                }

            }

        }
        static string Vuelta_al_prefijo(string ste) //Metodo solo para obtener la llave del nodo vista desde la raiz
        {
            //Le da vuelta a los prefijos
            char[] arrayVuelta = ste.ToCharArray();
            Array.Reverse(arrayVuelta);
            return new string(arrayVuelta);
        }
        List<LetraNodo> CrearPrefijos(List<ClaseNodo> ListaNodosArbol) //Funcion para setear los prefijos
        {
            List<LetraNodo> p = new List<LetraNodo>();
            //obtener nodos
            ClaseNodo NodoActual;
            int i = 0;
            for (i = 0; i < ListaNodosArbol.Count; i++)
            {
                NodoActual = ListaNodosArbol[i];
                //Armar el prefijo solo si el nodo es hoja
                if (NodoActual.Char_hijoDerecho == "" && NodoActual.Char_hijoIzquierdo == "")
                {
                    LetraNodo letra = new LetraNodo(); //creacion de una instancia letra para almacenar datos
                    NodoActual.codigo = Recorrer_arbol(NodoActual.caracter, ListaNodosArbol);
                    NodoActual.codigo = Vuelta_al_prefijo(NodoActual.codigo);
                    letra.caracter = NodoActual.caracter;
                    letra.llave = NodoActual.codigo;
                    letra.frecuencia = NodoActual.frecuencia; //Asignacion de la frecuencia del caracter
                    p.Add(letra);
                    ListaNodosArbol[i] = NodoActual; //actualizar registros en el arbol con sus prefijos ya establecidos
                }
            }
            return p;
        }
        string Recorrer_arbol(string CARACTERBASE, List<ClaseNodo> ListaNodosArbol) //Funcion Para recorrer el arbol y asignar los prefijos 
        {
            foreach (ClaseNodo item in ListaNodosArbol)
            {
                if (item.caracter == CARACTERBASE)
                {
                    //verificar que el nodo no sea raiz
                    if (item.Char_padre == "")
                        return "";
                    else //Obtiene el valor del camino del nodo y llama a la funcion misma pero para obtener el valor del padre
                        return item.camino + Recorrer_arbol(item.Char_padre, ListaNodosArbol);
                    break;

                }
            }
            return "";
        }
        public static void completarNodos1(List<ClaseNodo> ListaNodosArbol) //Termina de inicializar el nodo para cada caracter diferente
        {
            for (int i = 0; i < ListaNodosArbol.Count; i++)
            {
                if (ListaNodosArbol[i].Char_hijoDerecho == null)
                {
                    ClaseNodo a = new ClaseNodo();
                    a.caracter = ListaNodosArbol[i].caracter;
                    a.frecuencia = ListaNodosArbol[i].frecuencia;
                    a.camino = "";
                    a.codigo = "";
                    a.Char_padre = "";
                    a.Char_hijoIzquierdo = "";
                    a.Char_hijoDerecho = "";
                    ListaNodosArbol[i] = a;
                }
            }
        }

        //metodo para el arbol
        public static void JuntarNodos1(List<ClaseNodo> ListaNodosArbol) //Metodo para unir los nodos con mayor frecuencia y formar el arbol, este método da origen al arbol
        {
            //Contador unicamente para asignarle nombre a los nuevos nodos
            int NombreNodosNuevos = 0;
            int i;
            //Nodos a usar
            ClaseNodo NodoNuevo;
            ClaseNodo NodoActual;
            ClaseNodo NodoSiguiente;
            bool LlegarAlaRaiz = false; //verificador si el tamaño de la listanodosarbol es 1

            //inicializar el nodo nuevo

            NodoNuevo.caracter = "";
            NodoNuevo.frecuencia = 0;
            NodoNuevo.Char_padre = "";
            NodoNuevo.Char_hijoIzquierdo = "";
            NodoNuevo.Char_hijoDerecho = "";
            NodoNuevo.camino = "";
            NodoNuevo.codigo = "";

            while (!LlegarAlaRaiz)
            {
                //Crear el nodo padre con los dos nodos mayores
                for (i = 0; i < ListaNodosArbol.Count; i++)
                {
                    //solo queda un elemento en la raiz
                    if (i == ListaNodosArbol.Count - 1)
                    {
                        LlegarAlaRaiz = true;
                        break;
                    }
                    else
                    {
                        //obtener el nodo actual recorrido (el mayor)
                        NodoActual = ListaNodosArbol[i];
                        //Verificar que no tenga padre así dar prioridad a los nodos que poseen un caracter
                        if (NodoActual.Char_padre == "")
                        {
                            //obtener el nodo siguiente mas pequeño y juntarlos para crear un arbol pequeño
                            NodoSiguiente = ListaNodosArbol[i + 1];

                            //Crear el nombre del nodo nuevo N + i y crear el nodo
                            NombreNodosNuevos++;
                            NodoNuevo.caracter = "N" + NombreNodosNuevos;
                            NodoNuevo.frecuencia = NodoActual.frecuencia + NodoSiguiente.frecuencia;
                            NodoNuevo.Char_hijoIzquierdo = NodoActual.caracter;
                            NodoNuevo.Char_hijoDerecho = NodoSiguiente.caracter;

                            ListaNodosArbol.Add(NodoNuevo); //agregar el nodo nuevo al arbol

                            //actualizar nodos anteriores                            
                            NodoActual.Char_padre = NodoNuevo.caracter;
                            NodoActual.camino = "0";
                            NodoSiguiente.Char_padre = NodoNuevo.caracter;
                            NodoSiguiente.camino = "1";
                            //Meterlos con sus valores actuales al arbol
                            ListaNodosArbol[i] = NodoActual;
                            ListaNodosArbol[i + 1] = NodoSiguiente;

                            break;
                        }
                    }
                }
                if (!LlegarAlaRaiz)
                {
                    //Ordenar el arbol de nuevo
                    BubbleSort1(ListaNodosArbol);
                }
            }

        }

        public static void BubbleSort1(List<ClaseNodo> ArbolHUFF) //Ordenar los nodos del arbol segun la frecuencia
        {
            ClaseNodo xNodo;
            ClaseNodo yNodo;
            int ContadorNodo;
            int ContadorBubble;
            for (ContadorNodo = 0; ContadorNodo < ArbolHUFF.Count - 2; ContadorNodo++)
            {
                xNodo = ArbolHUFF[ContadorNodo];

                for (ContadorBubble = ContadorNodo + 1; ContadorBubble < ArbolHUFF.Count - 1; ContadorBubble++)
                {
                    yNodo = ArbolHUFF[ContadorBubble];

                    if (xNodo.frecuencia == yNodo.frecuencia)
                    {
                        break;
                    }

                    if ((xNodo.frecuencia != yNodo.frecuencia))
                    {
                        if ((xNodo.frecuencia > yNodo.frecuencia))
                        {
                            ArbolHUFF[ContadorNodo] = yNodo;
                            ArbolHUFF[ContadorBubble] = xNodo;
                            xNodo = yNodo;
                        }
                    }
                }

            }

        }
        public static string Vuelta_al_prefijo1(string ste) //Metodo solo para obtener la llave del nodo vista desde la raiz
        {
            //Le da vuelta a los prefijos
            char[] arrayVuelta = ste.ToCharArray();
            Array.Reverse(arrayVuelta);
            return new string(arrayVuelta);
        }
        public static List<LetraNodo> CrearPrefijos1(List<ClaseNodo> ListaNodosArbol) //Funcion para setear los prefijos
        {
            List<LetraNodo> p = new List<LetraNodo>();
            //obtener nodos
            ClaseNodo NodoActual;
            int i = 0;
            for (i = 0; i < ListaNodosArbol.Count; i++)
            {
                NodoActual = ListaNodosArbol[i];
                //Armar el prefijo solo si el nodo es hoja
                if (NodoActual.Char_hijoDerecho == "" && NodoActual.Char_hijoIzquierdo == "")
                {
                    LetraNodo letra = new LetraNodo(); //creacion de una instancia letra para almacenar datos
                    NodoActual.codigo = Recorrer_arbol1(NodoActual.caracter, ListaNodosArbol);
                    NodoActual.codigo = Vuelta_al_prefijo1(NodoActual.codigo);
                    letra.caracter = NodoActual.caracter;
                    letra.llave = NodoActual.codigo;
                    letra.frecuencia = NodoActual.frecuencia; //Asignacion de la frecuencia del caracter
                    p.Add(letra);
                    ListaNodosArbol[i] = NodoActual; //actualizar registros en el arbol con sus prefijos ya establecidos
                }
            }
            return p;
        }
        public static string Recorrer_arbol1(string CARACTERBASE, List<ClaseNodo> ListaNodosArbol) //Funcion Para recorrer el arbol y asignar los prefijos 
        {
            foreach (ClaseNodo item in ListaNodosArbol)
            {
                if (item.caracter == CARACTERBASE)
                {
                    //verificar que el nodo no sea raiz
                    if (item.Char_padre == "")
                        return "";
                    else //Obtiene el valor del camino del nodo y llama a la funcion misma pero para obtener el valor del padre
                        return item.camino + Recorrer_arbol1(item.Char_padre, ListaNodosArbol);
                }
            }
            return "";
        }
    }
}
