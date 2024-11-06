/*
 * Author FauZaPespi
 * Date: 06.11.2024
 * Détails: Execute du code C# dynamiquement
 */

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace P_Executor
{
    internal class Program
    {
        /// <summary>
        /// Méthode pour exécuter dynamiquement un code C# passé sous forme de chaîne de caractères.
        /// </summary>
        /// <param name="code">Le code à exécuter sous forme de chaîne</param>
        public static void ExecuteStringCode(string code)
        {
            // Création de la classe et méthode contenant le code à exécuter
            string classCode = @"
            using System;

            public class DynamicClass
            {
                public static void Execute()
                {
                    " + code + @"
                }
            }";

            // Initialisation du fournisseur de code C#
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            // Définition des paramètres du compilateur
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true, // Génère l'assemblage en mémoire sans fichier physique
                GenerateExecutable = false // Indique que l'assemblage est une bibliothèque et non un exécutable
            };

            // Ajoute une référence à System.dll, nécessaire pour le code C#
            parameters.ReferencedAssemblies.Add("System.dll");

            // Compilation du code de la classe
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, classCode);

            // Vérification des erreurs de compilation
            if (results.Errors.HasErrors)
            {
                // Affiche chaque erreur de compilation trouvée
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }
                throw new InvalidOperationException("Compilation failed."); // Lève une exception si la compilation échoue
            }

            // Obtient l'assemblage compilé en mémoire
            Assembly assembly = results.CompiledAssembly;
            // Récupère le type (classe) "DynamicClass"
            Type dynamicClass = assembly.GetType("DynamicClass");
            // Récupère la méthode "Execute" de la classe
            MethodInfo executeMethod = dynamicClass.GetMethod("Execute");
            // Exécute la méthode "Execute" sans passer d'instance (méthode statique)
            executeMethod.Invoke(null, null);
        }

        /// <summary>
        /// Méthode principale du programme, exécute du code dynamique et gère les interactions avec l'utilisateur
        /// </summary>
        /// <param name="args">Arguments passés en ligne de commande (non utilisés ici)</param>
        public static void Main(string[] args)
        {
            try
            {
                // Exécute un code de test simple pour afficher un message
                ExecuteStringCode("Console.WriteLine(\"Hello from dynamically compiled code!\");");
            }
            catch (Exception ex)
            {
                // Affiche l'erreur d'exécution si une exception est levée
                Console.WriteLine("Execution error: " + ex.Message);
            }
            Console.WriteLine("Hello from origine Code"); // Message indiquant l'origine du code
            Console.WriteLine("");

            try
            {
                // Exécute un autre exemple de code avec une boucle et un affichage de valeurs
                ExecuteStringCode("" +
                    "int a = 0;" +
                    "Console.WriteLine(a + \"->\");" +
                    "while (a < 10) {" +
                    "   a++;" +
                    "   Console.WriteLine(a);" +
                    "}" +
                    "Console.WriteLine(\"try\");" +
                    "");
            }
            catch (Exception ex)
            {
                // Affiche l'erreur d'exécution si une exception est levée
                Console.WriteLine("Execution error: " + ex.Message);
            }
            Console.WriteLine("Now it's your turn: "); // Demande à l'utilisateur d'entrer du code

            // Variable pour stocker le code que l'utilisateur souhaite exécuter
            string codeToExec = "";
            while (true)
            {
                // Lecture de la ligne entrée par l'utilisateur
                string ConsoleRl = Console.ReadLine();
                // Vérifie si l'utilisateur veut exécuter le code qu'il a saisi
                if (ConsoleRl == "execute->this")
                {
                    try
                    {
                        Console.Clear(); // Efface la console
                        ExecuteStringCode(codeToExec); // Exécute le code saisi par l'utilisateur
                    }
                    catch (Exception ex)
                    {
                        // Ignore l'erreur sans message (pourrait être amélioré pour afficher un message)
                    }
                }
                else
                {
                    // Ajoute la ligne entrée au code à exécuter
                    codeToExec += ConsoleRl;
                }
            }

            // Attente d'une touche pour fermer la console (n'est jamais atteint dans ce cas)
            Console.ReadKey();
        }
    }
}
