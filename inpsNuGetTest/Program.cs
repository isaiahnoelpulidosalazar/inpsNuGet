using inpsNuGet;

internal class Program
{
    private static void Main(string[] args)
    {
        CipherTest();
        STIMissionVisionTest();
    }

    static void CipherTest()
    {
        string a = "HELLO, WORLD!";
        string b = "CIPHER";
        string c = "OLSSV, DVYSK!";
        string d = "BEJJM, WMQJH!";
        string e = "IYRRD, ODJRX!";
        string f = "HLOWRDEL,OL!";
        string g = Cipher.CaesarCipher(a, 7);
        string h = Cipher.KeywordCipher(a, b);
        string i = Cipher.GiovanniCipher(a, b, "G");
        string j = Cipher.TranspositionCipher(a);
        Console.WriteLine("---\nCipher Test:");
        Console.WriteLine(g.Equals(c) ? "Test 1 Pass" : "Test 1 Fail");
        Console.WriteLine(h.Equals(d) ? "Test 2 Pass" : "Test 2 Fail");
        Console.WriteLine(i.Equals(e) ? "Test 3 Pass" : "Test 3 Fail");
        Console.WriteLine(j.Equals(f) ? "Test 4 Pass" : "Test 4 Fail");
        Console.WriteLine("---\n");
    }

    static void STIMissionVisionTest()
    {
        string a = "We are an institution committed to provide knowledge through the development and delivery of superior learning systems.";
        string b = "We strive to provide optimum value to all our stakeholders - our students, our faculty members, our employees, our partners, our shareholders, and our community.";
        string c = "We will pursue this mission with utmost integrity, dedication, transparency, and creativity.";
        string d = "To be the leader in innovative and relevant education that nurtures individuals to become competent and responsible members of society.";

        while (true)
        {
            Console.WriteLine("---\nMission:");
            Console.Write("1:");
            string? e = Console.ReadLine();
            if (!e.Equals(a))
            {
                Console.WriteLine("  " + a);
                Console.Write("Press any key to restart...");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.Write("2:");
                string? f = Console.ReadLine();
                if (!f.Equals(b))
                {
                    Console.WriteLine("  " + b);
                    Console.Write("Press any key to restart...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.Write("3:");
                    string? g = Console.ReadLine();
                    if (!g.Equals(c))
                    {
                        Console.WriteLine("  " + c);
                        Console.Write("Press any key to restart...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("\nVision:");
                        Console.Write("1:");
                        string? h = Console.ReadLine();
                        if (!h.Equals(d))
                        {
                            Console.WriteLine("  " + d);
                            Console.Write("Press any key to restart...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine("\nDone.\n---");
                            break;
                        }
                    }
                }
            }
        }
    }
}