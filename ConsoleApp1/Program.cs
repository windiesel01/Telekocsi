using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Telekocsi
{
    //Adatok

    //Auto
    public class AutoInfo
    {
        public string Indulas { get; set; }
        public string Cel { get; set; }
        public string Rendszam { get; set; }
        public string Telefonszam { get; set; }
        public int Ferohely { get; set; }
        public AutoInfo(string sor)
        {
            Cel = sor.Split(';')[0].Trim();
            Indulas = sor.Split(';')[1].Trim();
            Telefonszam = sor.Split(';')[3].Trim();
            Rendszam = sor.Split(';')[2].Trim();
            Ferohely = int.Parse(sor.Split(';')[4].Trim());
        }

    }

    //Igenyek
    public class IgenyInfo
    {
        public string Indulas { get; set; }
        public string Cel { get; set; }
        public string Azonosito { get; set; }
        public string Telefonszam { get; set; }
        public int Szemelyek { get; set; }
        public IgenyInfo(string line)
        {
            Cel = line.Split(';')[2].Trim();
            Indulas = line.Split(';')[1].Trim();
            Azonosito = line.Split(';')[0].Trim();
            Szemelyek = int.Parse(line.Split(';')[3].Trim());
        }

    }

    class Program
    {
        //Adatok tárlolása
        public static List<IgenyInfo> IgenyInfok = new List<IgenyInfo>();
        public static List<AutoInfo> AutoInfok = new List<AutoInfo>();
        //Beolvasás
        public static void Beolvas()
        {
            foreach (var item in File.ReadLines("autok.csv").Skip(1))
            {
                AutoInfok.Add(new AutoInfo(item));
            }
            foreach (var item in File.ReadLines("igenyek.csv").Skip(1))
            {
                IgenyInfok.Add(new IgenyInfo(item));
            }
        }

        static void Main(string[] args)
        {
            //1.FELADAT
            Console.WriteLine("1. feladat: Beolvasás"); Beolvas();
            //2.FELADAT
            Console.WriteLine($"\n\t2. feladat: {AutoInfok.GroupBy(a => a.Rendszam).Distinct().Count()} autós hirdetett fuvart.");
            //3.FELADAT
            int ossz = 0;
            foreach (var item in AutoInfok) 
            {
                if (item.Cel == "Miskolc" && item.Indulas == "Budapest")
                    ossz += item.Ferohely;
            }
            Console.WriteLine($"\n\t3. feladat: Összesen {ossz} férőhelyet hírdettek az autósok Budapestről -> Miskolcra.");
            //4.FELADAT
            AutoInfo max = AutoInfok[0];
            int maximum = AutoInfok[0].Ferohely;
            foreach (var item in AutoInfok)
            {
                if (item.Ferohely > maximum)
                {
                    max = item;
                    maximum = item.Ferohely;
                }
            }
            Console.WriteLine($"\n\t4. feladat: A legtöbb férőhely a {max.Indulas} - {max.Cel} között volt {max.Ferohely} férőhellyel.");
            //5.FELADAT
            Dictionary<IgenyInfo, AutoInfo> Egyezes = new Dictionary<IgenyInfo, AutoInfo>();
            foreach (var igeny in IgenyInfok)
            {
                foreach (var jarat in AutoInfok)
                {
                    if (!(Egyezes.ContainsKey(igeny)) &&
                        (igeny.Cel == jarat.Cel && igeny.Indulas == jarat.Indulas && igeny.Szemelyek <= jarat.Ferohely))
                    {
                        Egyezes.Add(igeny, jarat);
                    }
                }
            }
            Console.WriteLine("\n\t5. feladat:");
            foreach (var item in Egyezes)
            {
                Console.WriteLine("\t " + item.Key.Azonosito + " => " + item.Value.Rendszam);
            }
            //6.FELADAT
            Console.WriteLine("\n\t6. feladat: utasuzenetek.txt");
            using (StreamWriter kiiratas = new StreamWriter("utasuzenetek.txt"))
            {
                foreach (var item in IgenyInfok)
                {
                    if (Egyezes.ContainsKey(item))
                    {
                        kiiratas.WriteLine(item.Azonosito + $": Rendszám: {Egyezes[item].Rendszam}, Telefonszám: {Egyezes[item].Telefonszam}");
                    }
                    else
                    {
                        kiiratas.WriteLine(item.Azonosito + ": Sajnos nem sikerült autót találni");
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
