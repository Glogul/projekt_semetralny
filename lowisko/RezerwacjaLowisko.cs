using System;
using System.Collections.Generic;
using System.IO;

class Stanowisko
{
    public int Numer { get; set; }
    public int MiejscaDzien { get; set; }
    public bool DostepneDzien { get; set; }
    public int MiejscaNoc { get; set; }
    public bool DostepneNoc { get; set; }
}

class Program
{
    static List<Stanowisko> stanowiska = new List<Stanowisko>();
    static string plikDanych = "lowisko.txt";

    static void Main(string[] args)
    {
        WczytajDane();

        // Krok 1: Powitanie
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║     Witamy z tej strony łowisko Okoń     ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Krok 2 & 3: Wybór pory dnia
        bool czyDzien = WybierzPore();

        // Krok 4: Wyświetl dostępność stanowisk
        WyswietlDostepnosc(czyDzien);

        // Krok 5: Wybór stanowiska i liczby osób
        int wybraneStanowisko;
        int liczbaOsob;
        WybierzStanowisko(czyDzien, out wybraneStanowisko, out liczbaOsob);

        // Krok 6: Podanie imienia i nazwiska
        Console.WriteLine();
        Console.Write("Podaj imię i nazwisko, aby zakończyć proces rezerwacji: ");
        string imieNazwisko = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(imieNazwisko))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Imię i nazwisko nie może być puste. Spróbuj ponownie.");
            Console.ResetColor();
            Console.Write("Podaj imię i nazwisko: ");
            imieNazwisko = Console.ReadLine();
        }

        // Krok 7: Potwierdzenie rezerwacji
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║          Rezerwacja przebiega pomyślnie!             ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════╣");
        Console.WriteLine($"║  Rezerwujący : {imieNazwisko,-38}║");
        Console.WriteLine($"║  Pora        : {(czyDzien ? "Dzień" : "Noc"),-38}║");
        Console.WriteLine($"║  Stanowisko  : {wybraneStanowisko,-38}║");
        Console.WriteLine($"║  Liczba osób : {liczbaOsob,-38}║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine();
        Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();
    }

    static void WczytajDane()
    {
        if (!File.Exists(plikDanych))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"BŁĄD: Nie znaleziono pliku '{plikDanych}'!");
            Console.WriteLine("Upewnij się, że plik lowisko.txt znajduje się w tym samym folderze co program.");
            Console.ResetColor();
            Console.ReadKey();
            Environment.Exit(1);
        }

        stanowiska.Clear();
        string[] linie = File.ReadAllLines(plikDanych);

        foreach (string linia in linie)
        {
            // Pomijaj komentarze i puste linie
            if (linia.StartsWith("#") || string.IsNullOrWhiteSpace(linia))
                continue;

            string[] czesci = linia.Split(';');
            if (czesci.Length < 5) continue;

            try
            {
                Stanowisko s = new Stanowisko
                {
                    Numer = int.Parse(czesci[0].Trim()),
                    MiejscaDzien = int.Parse(czesci[1].Trim()),
                    DostepneDzien = czesci[2].Trim().ToLower() == "tak",
                    MiejscaNoc = int.Parse(czesci[3].Trim()),
                    DostepneNoc = czesci[4].Trim().ToLower() == "tak"
                };
                stanowiska.Add(s);
            }
            catch
            {
                // Pomijaj błędne linie
            }
        }
    }

    static bool WybierzPore()
    {
        while (true)
        {
            Console.Write("Proszę wybrać porę rezerwacji (Dzień/Noc): ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "dzień" || input == "dzien" || input == "d")
                return true;
            else if (input == "noc" || input == "n")
                return false;
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nieprawidłowy wybór! Wpisz 'Dzień' lub 'Noc'.");
                Console.ResetColor();
            }
        }
    }

    static void WyswietlDostepnosc(bool czyDzien)
    {
        string pora = czyDzien ? "DZIEŃ" : "NOC";
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  Dostępność stanowisk - {pora}");
        Console.WriteLine("  ┌────────────┬──────────────┬─────────────┐");
        Console.WriteLine("  │ Stanowisko │ Liczba miejsc│ Dostępność  │");
        Console.WriteLine("  ├────────────┼──────────────┼─────────────┤");
        Console.ResetColor();

        foreach (var s in stanowiska)
        {
            int miejsca = czyDzien ? s.MiejscaDzien : s.MiejscaNoc;
            bool dostepne = czyDzien ? s.DostepneDzien : s.DostepneNoc;
            string status = dostepne ? "Dostępne" : "Zajęte";

            if (dostepne)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"  │     {s.Numer,2}     │      {miejsca,2}       │  {status,-11}│");
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  └────────────┴──────────────┴─────────────┘");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void WybierzStanowisko(bool czyDzien, out int wybraneNr, out int wybranaLiczba)
    {
        wybraneNr = 0;
        wybranaLiczba = 0;

        while (true)
        {
            // Wybór numeru stanowiska
            Console.Write("Wybierz numer stanowiska (1-10): ");
            string inputStan = Console.ReadLine()?.Trim();
            int numerStanowiska;

            if (!int.TryParse(inputStan, out numerStanowiska) || numerStanowiska < 1 || numerStanowiska > 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nieprawidłowy numer stanowiska! Wpisz liczbę od 1 do 10.");
                Console.ResetColor();
                continue;
            }

            Stanowisko wybrane = stanowiska.Find(s => s.Numer == numerStanowiska);
            if (wybrane == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nie znaleziono stanowiska o podanym numerze.");
                Console.ResetColor();
                continue;
            }

            bool dostepne = czyDzien ? wybrane.DostepneDzien : wybrane.DostepneNoc;
            int maxMiejsc = czyDzien ? wybrane.MiejscaDzien : wybrane.MiejscaNoc;

            if (!dostepne)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Stanowisko {numerStanowiska} jest niedostępne w tej porze. Wybierz inne.");
                Console.ResetColor();
                continue;
            }

            // Wybór liczby osób
            Console.Write($"Podaj liczbę osób (max {maxMiejsc}): ");
            string inputOsoby = Console.ReadLine()?.Trim();
            int liczbaOsob;

            if (!int.TryParse(inputOsoby, out liczbaOsob) || liczbaOsob < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nieprawidłowa liczba osób! Wpisz liczbę większą od 0.");
                Console.ResetColor();
                continue;
            }

            if (liczbaOsob > maxMiejsc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Liczba osób ({liczbaOsob}) przekracza liczbę miejsc na stanowisku {numerStanowiska} ({maxMiejsc}).");
                Console.WriteLine("Proszę wybrać stanowisko i liczbę osób ponownie.");
                Console.ResetColor();
                Console.WriteLine();
                continue;
            }

            // Wszystko OK
            wybraneNr = numerStanowiska;
            wybranaLiczba = liczbaOsob;
            break;
        }
    }
}
