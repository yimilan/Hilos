using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Collections;
using System.Threading;

namespace Decrypting_CMD
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Origin obj = new Origin ();
			obj.origin ();

		}
	}

	public class Origin{
		/*
		 * @param Variables Universales
		 */
		static int TAM_MAX = 100;
		String[] user = new String[TAM_MAX];
		String[] paswordHash = new String[TAM_MAX];
		int cant = 0;
		static internal Thread[] threadAtack;
		public int controlChar = 0;
		char cambChar = 'a';
		char A = 'A';
		char Z = 'Z';
		char a = 'a';
		char z = 'z';
		public char[] palabra = new char[4];
		bool aum;
		String D = "Z";
		String Dic = "Z";
		int reg = 0;

		protected const int MAX_THREADS = 8;
		protected const int MAX_CHARS = 4;
		private string current_word = "";
		private string[] passwords;


		/*
		 * @param Metodo para administrar todos los procesos
		 */
		public void origin(){
			read ();
			imprimir ();
			init ();
			Console.ReadKey ();
			Console.Clear ();
			/*do {
				String d;
				d = getNewWord();
				Console.WriteLine(d);
			} while(String.Compare (D, "ZZZZZ") != 0);*/
			processes ();
			Console.WriteLine ("#### Crack Exitoso ####");
			Console.ReadKey ();
		}

		/*
		 * @param Metodo crackingThread
		 */
		public void crackingThread(int thread, bool active, string password, string result) {
			bool end = false;
			while (!end) {
				if (active) {
					result = generarHash(password);
					Console.WriteLine (result);
				} else {
					Thread.Sleep(1000); // ...
				}
			}
		}

		/*
		 * @param Metodo main del ataque
		 */
		public void processes(){
			read ();
			imprimir ();
			Console.ReadKey ();
			Console.Clear ();

			// Variables compartidas
			//int[] threads = new int[MAX_THREADS];
			bool[] active = new bool[MAX_THREADS];	// Control: Processes pone a true para que thread empiece proceso. Cuando acaba pone a 0 de nuevo.
			string[] password = new string[MAX_THREADS];	// Hash que se analiza
			string[] result = new string[MAX_THREADS];	// Resultado.
			Thread[] threads = new Thread[MAX_THREADS];
			String[] comm = new string[MAX_CHARS];
			// Generar Threads y lanzar con variables compartidas
			for (int thread = 0; thread < MAX_CHARS; thread++) {
				// Lanzar Threads
				threads[thread]= new Thread (new ThreadStart (() => crackingThread(thread, active[thread], password[thread], result[thread])));
				threads[thread].Start();
			}

			// Bucle Hack
			bool end = false;
			while (end != true) {
				for (int thread = 0; thread < MAX_CHARS; thread++) {
					Console.WriteLine (password[thread]);
					if (active[thread] == false) {
						// Para primera interacción
						if (comm[thread] == "" || active[thread] == false) {
							if (comm[thread] != "") {
								// Recojo resultado
								// comparamos este hash con todos los hashes que tenemos cargados
								for (int word = 0; word < cant; word++) {
									if (this.paswordHash[word] == result[thread]) {	// result es password encriptada
										this.passwords[word] = password[thread];
										Console.WriteLine (password[thread]);
										reg++;
									}
								}
							}
							if (!allWordsCracked()) {	// Aún no hemos encontrado todas las claves, se revisan de this.
								// Pido palabra a comparar (generada por nosotros)
								current_word = getNewWord();
								// paso palabra al thread
								comm[thread] = current_word;
								active[thread] = true;
							}
						}
					}
				}
				if (allWordsCracked()) {
					end = true;
				} else {
					Thread.Sleep (1000);	// Dormir 1 segundo. Ajustar a lo suficiente como para que más o menos coincida con generar un hash.
					// No se cómo se hace sleep en C#
				}
			}
			Console.WriteLine ("#### Crack Exitoso ####");
			for (int i = 0; i < cant; i++) {
				Console.WriteLine ("Hash: " + this.paswordHash[i] + ", contraseña: " + this.passwords[i]);
			}
			Console.ReadKey ();
		}

		public bool allWordsCracked(){
			if (reg == cant) {
				return true;
			}
			return false;
		}

		/*private getNewWord(string word) {
			char a = 'a';
			char z = 'z';
			char A = 'A';
			char Z = 'Z';
			//char Ñ = 'Ñ'; // No se si está en ASCII, pero bueno

			// tienes una palabra, por ejemplo gdf
			// buscas de derecha a izquierda, sumar un 1 al caracter de la derecha. Si es z pones A. Si es Z pones a y aumentas el de la izda.
			// si llego al final, (ZZZ), añado un caracter y reseteo (aaaa)
			// No se si me explico. Si encuentro a pongo b. Si b pongo c. si z pongo A. Si Z pongo aa si fZ pongo ga, etc.
			// Si llego a ZZZZ devuelvo null y se acaba. hay que controlar el máximo con la constante.
		}*/

		/*
        * @param Metodo para cifrar
        */
		public String generarHash(String bruteF){
			MD5 md5 = MD5CryptoServiceProvider.Create();
			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] stream = null;
			StringBuilder sb = new StringBuilder();
			stream = md5.ComputeHash(encoding.GetBytes(bruteF));
			for (int i = 0; i < stream.Length; i++) {
				sb.AppendFormat ("{0:x2}", stream [i]);
			}
			Console.WriteLine (sb.ToString());
			return sb.ToString();
		}

		/*
        * @param Metodo para crear diccionario de ataque
        */
	

			/*
        * @param Metodo para apertura y lectura de archivo
         * construcion de ArrayList
        */
		public void read()
		{
			string line;
			String ruta;
			//Console.Write ("Introduzca la ruta o arrastre el archivo a la consola para obtener ruta: ");
			//ruta = Console.ReadLine ();
			//Console.WriteLine ();
			System.IO.StreamReader file = new System.IO.StreamReader(@"user.txt");
			while((line = file.ReadLine()) != null)
			{
				
			Console.WriteLine ("Encrypting: " + line);
			breakString (line);
			cant++;


				//Console.WriteLine ("Cargado hash: " + line);
			}
			file.Close();
		}

			/*
        * @param Metodo para Romper Lineas
        */
			public void breakString(string line){
				char[] delimit = new char[] {':'};
				int cont = 0;
				foreach (String subStr in line.Split(delimit)) {
					if (cont == 2) {					
						Console.WriteLine("Password: " + subStr);
						paswordHash [cant] = subStr;

					} else {
						if (cont == 0) {
							Console.WriteLine ("User: " + subStr);
							user [cant] = subStr;
						}
					}
					cont++;
				}
				Console.WriteLine ();
			}
			/*
        * @param Metodo para imprimir contendo user
        */
			public void imprimir(){
				for (int i = 0; i < cant; i++)
				{
					Console.Write (user [i] + "\t");
					//Console.WriteLine (paswordHash[i]);
				}
			}
			/*
        * @param Metodo para comparar
        */
			public bool compare(String passMD5, ref int pos){
				for (int i = 0; i < cant; i++) {
					//Console.WriteLine (paswordHash [i] + " " + passMD5);
					if(String.Compare(paswordHash[i],passMD5) == 0){
						pos = i;
						return true;
					}
				}
				return false;
			}

			/*
        * @param Metodo para mostrar user Crack
        */
			public void look(String pass, int pos){

				Console.WriteLine (user[pos] + "\t" + pass);
				//Console.WriteLine ("_____________________________________________");

			}

			public string getNewWord(){
				String dicc;
				dicc = diccionario ();
				Console.WriteLine (dicc);
				if(String.Compare(dicc, Dic)==0){
					palabra[controlChar - 1] = 'a';
					Dic = Dic + "Z";
				}
				return dicc;
			}

			public void init(){
				for (int i = 0; i < MAX_CHARS; i++) {
					palabra[i] = '\0';
				}
				palabra[0] = cambChar;
			}

			public String diccionario(){
				aum = false;
				palabra[controlChar] = cambChar;
				string s = new string(palabra);
				if (palabra [controlChar] == z) {
					//aum = true;
					cambChar = A;					
				} 
				if (palabra [controlChar] == Z) {
					cambChar = a;
					cambio (controlChar);
					aum = true;
				} 
				if (String.Compare (s, D) == 0) {
					D = D + "Z";
					controlChar++;
				}
				if (aum == false) {
					cambChar++;
				}
				return s;
			}

			public void cambio(int cC){
				if (palabra [cC] == Z) {
					palabra [cC] = a;

					if (cC > 0) {
						if (palabra [cC - 1] == Z) {
							palabra [cC - 1] = a;
							//............................
							if (cC > 1) {
								if (palabra [cC - 2] == Z) {
									palabra [cC - 2] = a;
									//..........................................
									if (cC > 2) {
										if (palabra [cC - 3] == Z) {
											palabra [cC - 3] = a;
										} else {
											if (palabra [cC - 3] == z) {
												palabra [cC - 3] = A;
											} else {
												palabra [cC - 3]++;
											}
										}
									}
									//..........................................
								} else {
									if (palabra [cC - 2] == z) {
										palabra [cC - 2] = A;
									} else {
										palabra [cC - 2]++;
									}
								}
							}
							//................................
						} else {
							if (palabra [cC - 1] == z) {
								palabra [cC - 1] = A;
							} else {
								palabra [cC - 1]++;
							}
						}
					}
				}

			}
		}
	}