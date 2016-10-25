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

			obj.processes ();
			Console.WriteLine ("#### Crack Exitoso ####");
			Console.ReadKey ();

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
		int cantAtaq; 
		protected const int MAX_THREADS = 8;
		protected const int MAX_CHARS = 4;
		private string current_word = "";
		private string[] passwords;



		/*
		 * @param Metodo crackingThread
		 */
		public static void crackingThread(int thread,ref bool[] active, ref string[] password, ref string[] result, ref bool[] end) {
			Console.WriteLine (password[thread]);
			while (!end[thread]) {
				if (active[thread]) {
					result[thread] = generarHash(password[thread]);
				} else {
					Thread.Sleep(500); // ...
				}
			}
		}

		/*
		 * @param Metodo main del ataque
		 */

		static bool[] active = new bool[MAX_THREADS];	
		static bool[] end = new bool[MAX_THREADS];
		static string[] password = new string[MAX_THREADS];
		static string[] result = new string[MAX_THREADS];

		public void processes(){
			read ();
			init ();
			Console.Clear ();
			/*bool[] active = new bool[MAX_THREADS];	
			bool[] end = new bool[MAX_THREADS];
			string[] password = new string[MAX_THREADS];
			string[] result = new string[MAX_THREADS];*/
			Thread[] threads = new Thread[MAX_THREADS];
			for (int thread = 0; thread < cantAtaq; thread++) {
				//threads[thread]= new Thread (new ThreadStart (() =>crackingThread(thread,ref active,ref password,ref result, ref end)));

				threads[thread]= new Thread (new ThreadStart (() =>{
					Console.WriteLine (password[thread]);
					while (!end[thread]) {
						if (active[thread]) {
							result[thread] = generarHash(password[thread]);
						} else {
							Thread.Sleep(500); // ...
						}
					}
				}));
				threads[thread].Start();
			}
			bool end1 = false;
			while (end1 != true) {	
				for (int thread = 0; thread < cantAtaq; thread++) {
					if (active[thread] == false) {
						if (password[thread] == "" || active[thread] == false) {							
							if (password[thread] != "") {								
								for (int word = 0; word < cant; word++) {									
									if (this.paswordHash[word] == result[thread]) {
										this.passwords[word] = password[thread];
										reg++;
									}
								}
							}
							if (!allWordsCracked()) {	
								current_word = getNewWord();
								password[thread] = current_word;
								active[thread] = true;
							}
						}
					}
				}
				if (allWordsCracked()) {
					end1 = true;
				} else {
					Thread.Sleep (500);
				}
			}
			for (int i = 0; i < cant; i++) {
				Console.WriteLine ("Hash: " + this.paswordHash[i] + ", contraseña: " + this.passwords[i]);
			}
			Console.ReadKey ();
		}

		//..........................................
		public bool allWordsCracked(){
			if (reg == cant) {
				return true;
			}
			return false;
		}

		/*
        * @param Metodo para cifrar
        */
		public static String generarHash(String bruteF){
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
			Console.Write ("Ingrese cantidad de hilos a realizar en el ataque: ");
			cantAtaq = Convert.ToInt32(Console.ReadLine ());
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
			//Console.WriteLine (dicc);
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