using System.Text.RegularExpressions;

namespace dz12
{
    class Poem
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Text { get; set; }
        public string Theme { get; set; }

        public override string ToString()
        {
            return $"Nazva: {Title}\nAvtor: {Author}\nRik: {Year}\nTema: {Theme}\nTekst: {Text}\n";
        }
    }

    class Program
    {
        static List<Poem> poems = new List<Poem>();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== Menyu ===");
                Console.WriteLine("1. Dodaty virsh");
                Console.WriteLine("2. Vydalyty virsh");
                Console.WriteLine("3. Redahuvaty virsh");
                Console.WriteLine("4. Poshuk");
                Console.WriteLine("5. Zberehty u fayl");
                Console.WriteLine("6. Zavantažyty z faylu");
                Console.WriteLine("7. Zvit");
                Console.WriteLine("0. Vykhid");
                Console.Write("Vybir: ");

                switch (Console.ReadLine())
                {
                    case "1": AddPoem(); break;
                    case "2": RemovePoem(); break;
                    case "3": EditPoem(); break;
                    case "4": SearchPoem(); break;
                    case "5": SaveToFile(); break;
                    case "6": LoadFromFile(); break;
                    case "7": GenerateReport(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Nevírnyy vybir."); break;
                }
            }
        }

        static void AddPoem()
        {
            var poem = new Poem();
            Console.Write("Nazva: "); poem.Title = Console.ReadLine();
            Console.Write("Avtor: "); poem.Author = Console.ReadLine();
            Console.Write("Rik: "); poem.Year = int.Parse(Console.ReadLine());
            Console.Write("Tema: "); poem.Theme = Console.ReadLine();
            Console.Write("Tekst: "); poem.Text = Console.ReadLine();

            poems.Add(poem);
            Console.WriteLine("Virsh dodano.");
        }

        static void RemovePoem()
        {
            Console.Write("Vveditʹ nazvu virsha dlya vydalennya: ");
            string title = Console.ReadLine();
            var poem = poems.FirstOrDefault(p => p.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (poem != null)
            {
                poems.Remove(poem);
                Console.WriteLine("Virsh vydaleno.");
            }
            else
            {
                Console.WriteLine("Virsh ne znaydeno.");
            }
        }

        static void EditPoem()
        {
            Console.Write("Vveditʹ nazvu virsha dlya redahuvannya: ");
            string title = Console.ReadLine();
            var poem = poems.FirstOrDefault(p => p.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (poem != null)
            {
                Console.Write("Nova nazva (zalyshte porozhnim dlya propusku): ");
                string newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle)) poem.Title = newTitle;

                Console.Write("Novyy avtor: ");
                string newAuthor = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAuthor)) poem.Author = newAuthor;

                Console.Write("Novyy rik: ");
                string yearInput = Console.ReadLine();
                if (int.TryParse(yearInput, out int newYear)) poem.Year = newYear;

                Console.Write("Nova tema: ");
                string newTheme = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTheme)) poem.Theme = newTheme;

                Console.Write("Novyy tekst: ");
                string newText = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newText)) poem.Text = newText;

                Console.WriteLine("Virsh onovleno.");
            }
            else
            {
                Console.WriteLine("Virsh ne znaydeno.");
            }
        }

        static void SearchPoem()
        {
            Console.WriteLine("Shukaty za: 1. Nazvoyu  2. Avtorom  3. Temoyu  4. Slovom u teksti  5. Rokom");
            string choice = Console.ReadLine();

            Console.Write("Vveditʹ znachennya dlya poshuku: ");
            string query = Console.ReadLine();

            IEnumerable<Poem> results = choice switch
            {
                "1" => poems.Where(p => p.Title.Contains(query, StringComparison.OrdinalIgnoreCase)),
                "2" => poems.Where(p => p.Author.Contains(query, StringComparison.OrdinalIgnoreCase)),
                "3" => poems.Where(p => p.Theme.Contains(query, StringComparison.OrdinalIgnoreCase)),
                "4" => poems.Where(p => p.Text.Contains(query, StringComparison.OrdinalIgnoreCase)),
                "5" => poems.Where(p => p.Year.ToString() == query),
                _ => Enumerable.Empty<Poem>()
            };

            foreach (var poem in results)
            {
                Console.WriteLine(poem);
            }
        }

        static void SaveToFile()
        {
            using (StreamWriter writer = new StreamWriter("poems.txt"))
            {
                foreach (var poem in poems)
                {
                    writer.WriteLine($"{poem.Title}|{poem.Author}|{poem.Year}|{poem.Theme}|{poem.Text}");
                }
            }
            Console.WriteLine("Kolektsiya zberezhena u fayl.");
        }

        static void LoadFromFile()
        {
            if (!File.Exists("poems.txt"))
            {
                Console.WriteLine("Fayl ne znaydeno.");
                return;
            }

            poems.Clear();
            foreach (var line in File.ReadAllLines("poems.txt"))
            {
                var parts = line.Split('|');
                if (parts.Length == 5)
                {
                    poems.Add(new Poem
                    {
                        Title = parts[0],
                        Author = parts[1],
                        Year = int.Parse(parts[2]),
                        Theme = parts[3],
                        Text = parts[4]
                    });
                }
            }
            Console.WriteLine("Kolektsiya zavantazheno z faylu.");
        }

        static void GenerateReport()
        {
            Console.WriteLine("Zvit za: 1. Nazvoyu 2. Avtorom 3. Temoyu 4. Slovom u teksti 5. Rokom 6. Dovzhynoyu virsha");
            string choice = Console.ReadLine();

            IEnumerable<IGrouping<object, Poem>> report = choice switch
            {
                "1" => poems.GroupBy(p => (object)p.Title),
                "2" => poems.GroupBy(p => (object)p.Author),
                "3" => poems.GroupBy(p => (object)p.Theme),
                "4" => poems.GroupBy(p => (object)p.Text),
                "5" => poems.GroupBy(p => (object)p.Year),
                "6" => poems.GroupBy(p => (object)p.Text.Length),
                _ => null
            };

            if (report != null)
            {
                foreach (var group in report)
                {
                    Console.WriteLine($"\n=== {group.Key} ===");
                    foreach (var poem in group)
                    {
                        Console.WriteLine(poem);
                    }
                }
                Console.WriteLine("Zvit hotovyy.");
            }
            else
            {
                Console.WriteLine("Nevírnyy vybir.");
            }
        }
    }
}
