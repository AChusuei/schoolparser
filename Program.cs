using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FileHelpers;
using AutoMapper;

namespace FileTransform
{
    public class Program
    {
        private static FileHandlerFactory factory;

        public static void Main(string[] args)
        {
            factory = new FileHandlerFactory();
            if (!ValidateInput(args)) return;
            var firsthandler = factory.GetFileHandler(Path.GetExtension(args[0]));
            firsthandler.ReadFile(args[0]);
            var secondhandler = factory.GetFileHandler(Path.GetExtension(args[1]));
            // With this framework, one should be able to create a new type of IFileHandler, and dynamically map onto different IFileHandlers.
            firsthandler.MapStoreOnto(secondhandler);
            secondhandler.WriteFile(args[1]);
            Console.WriteLine("File transformation complete (from {0} to {1}).", firsthandler.FileType, secondhandler.FileType);
        }

        // This just validates the command line input, nothing mysterious here.
        private static bool ValidateInput(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("You do not have exactly two arguments");
                return false;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("First file {0} does not exist!", args[0]);
                return false;
            }

            if (String.IsNullOrWhiteSpace(Path.GetExtension(args[0]))) // no extension
            {
                Console.WriteLine("Cannot retrieve extension from first file '{0}'!", args[0]);
                return false;
            }
            if (factory.GetFileHandler(Path.GetExtension(args[0])) == null) // no handler for this extension
            {
                Console.WriteLine("Cannot retrieve file handler for first file '{0}'!", Path.GetExtension(args[0]));
                return false;
            }

            if (String.IsNullOrWhiteSpace(Path.GetExtension(args[1]))) // no extension
            {
                Console.WriteLine("Cannot retrieve extension from second file '{0}'!", args[1]);
                return false;
            }
            if (Path.GetExtension(args[0]) == Path.GetExtension(args[1])) // same extension!
            {
                Console.WriteLine("Path extensions are the same for both files '{0}'!", Path.GetExtension(args[0]));
                return false;
            }
            if (factory.GetFileHandler(Path.GetExtension(args[1])) == null) // no handler for this extension
            {
                Console.WriteLine("Cannot retrieve file handler for second file '{0}'!", Path.GetExtension(args[1]));
                return false;
            }

            return true;
        }
    }
}
