using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Google_Hashcode_2019
{
    class Program
    {
        static void Main(string[] args)
        {


            new Thread(() =>
            {
                parseAFile(@"C:\Users\Flavien\Downloads\a_example.txt", @"C:\Users\Flavien\Desktop\a.txt");
            }).Start();

            new Thread(() =>
            {
                parseAFile(@"C:\Users\Flavien\Downloads\b_lovely_landscapes.txt", @"C:\Users\Flavien\Desktop\b.txt");
            }).Start();

            new Thread(() =>
            {
                parseAFile(@"C:\Users\Flavien\Downloads\c_memorable_moments.txt", @"C:\Users\Flavien\Desktop\c.txt");
            }).Start();

            new Thread(() =>
            {
                parseAFile(@"C:\Users\Flavien\Downloads\d_pet_pictures.txt", @"C:\Users\Flavien\Desktop\d.txt");
            }).Start();

            new Thread(() =>
            {
                parseAFile(@"C:\Users\Flavien\Downloads\e_shiny_selfies.txt", @"C:\Users\Flavien\Desktop\e.txt");
            }).Start();




        }

        public static void parseAFile(string filename, string fileOut)
        {
            List<string> slides = new List<string>();
            List<Photo> photo_list = parseFile(filename);

            sortListViaNbTag(photo_list);
            List<Slide> slideshow = ToSlideShowV2(photo_list);
            string sub = createSubmission(slideshow);

            System.IO.File.WriteAllText(fileOut, sub);
            Console.WriteLine(sub);
        }

        public static List<Photo> parseFile(string filename)
        {
            string text = File.ReadAllText(filename, Encoding.UTF8);
            string[] liste = text.Split('\n');

            // Nombre d'image
            string nbr_image = liste[0];

            int index = 0;

            List<Photo> a = new List<Photo>();
            for(int i = 1; i < liste.Length; i++)
            {
                string[] splited = liste[i].Split();

                if (splited[0] == "")
                    continue;

                Photo p = new Photo();
                p.orientation = char.Parse(splited[0]);
                p.nbrTags = int.Parse(splited[1]);
                p.index = index;
                for(int j = 2; j <= splited.Length-1; j++)
                {
                    p.tags.Add(splited[j]);
                }
                a.Add(p);
                index++;
            }

            Console.WriteLine("Parser " + a.Count + " images:");
            /*foreach(Photo p in a)
            {
                Console.WriteLine("Orientation: " + p.orientation + " - Nb Tags: " + p.nbrTags);
                foreach (string tag in p.tags)
                    Console.Write(tag + " ");
                Console.WriteLine("");
            }*/

            return a;
            
        }

        public static string createSubmission(List<Slide> liste_slide)
        {
            string sub = "";
            int nb_slides = liste_slide.Count;
            sub = nb_slides + "\n";

            foreach(Slide s in liste_slide)
            {
                if (s.photos.Count == 2)
                    sub += s.photos[0].index + " " + s.photos[1].index;
                else
                    sub += s.photos[0].index;

                sub += "\n";
            }
            sub = sub.Substring(0, sub.Length - 1);
            return sub;
        }

        public static void sortList(List<Photo> H, List<Photo> V, List<Photo> entry)
        {
            foreach(Photo p in entry)
            {
                if (p.orientation == 'H')
                    H.Add(p);
                else
                    V.Add(p);
            }
        }

        static int coef(Photo a, Photo b)
        {
            int tag_commun = 0;

            foreach(string tag1 in a.tags)
            {
                foreach(string tag2 in b.tags)
                {
                    if(tag1 == tag2)
                    {
                        tag_commun++;
                        break;
                    }
                }
            }

            return tag_commun;
        }

        public static void sortListViaNbTag(List<Photo> a)
        {
            a.Sort((x, y) => y.nbrTags.CompareTo(x.nbrTags)); // desc
        }

        public static List<Slide> ToSlideShow(List<Photo> a)
        {
            List<Slide> slideshow = new List<Slide>();

            List<Photo> H = new List<Photo>();
            List<Photo> V = new List<Photo>();

            sortList(H, V, a);

            int j = 0;
            for(int i = 0; i < a.Count; i++)
            {
                if (a[i].orientation == 'H')
                {
                    Slide s1 = new Slide();
                    s1.photos.Add(a[i]);
                    
                    slideshow.Add(s1);
                }
                else
                {

                    if (j >= V.Count / 2)
                        continue;

                    Slide s = new Slide();
                    s.photos.Add(a[i]);
                    s.photos.Add(V[V.Count - 1 - j]);
                    slideshow.Add(s);
                    j++;
                }

            }

            return slideshow;
        }

        public static List<Slide> ToSlideShowV2(List<Photo> a)
        {
            List<Slide> slideshow = new List<Slide>();

            int i = 0;
            int indexBestMatch = 0;
            while (a.Count != 0)
            {
                Photo prend = a[i];

                if(prend.orientation == 'H')
                {
                    indexBestMatch = searchBestCoefH(a, i);

                    Slide s1 = new Slide();
                    s1.photos.Add(prend);
                    slideshow.Add(s1);
                    a.RemoveAt(i);

                    if (i < indexBestMatch)
                        i = indexBestMatch - 1;
                    else
                        i = indexBestMatch;
                }
                else
                {
                    // worst match
                    int worst = searchWorstCoefV(a, i);
                    Slide s = new Slide();
                    s.photos.Add(a[i]);
                    s.photos.Add(a[worst]);
                    s.concatTagsPhotos();

                    a.RemoveAt(i);

                    if (i < worst)
                        worst--;

                    a.RemoveAt(worst);

                    slideshow.Add(s);

                    // best coef slide
                    int BestIndex = searchBestCoefSlide(s, a, i);

                    if (i < BestIndex)
                        i = BestIndex - 2;
                    else
                        i = BestIndex;
                }
            }

            return slideshow;
        }


        // Return index of the list
        public static int searchBestCoefH(List<Photo> a, int indexToSearch)
        {
            int index = 0;
            int maxCoef = 0;
            Photo toCompare = a[indexToSearch];

            for(int i = 0; i < a.Count; i++)
            {
                if(i != indexToSearch && a[i].orientation != 'V' && coef(toCompare, a[i]) > maxCoef)
                {
                    maxCoef = coef(toCompare, a[i]);
                    index = i;
                }
            }

            return index;
        }

        public static int searchBestCoef(List<Photo> a, int indexToSearch)
        {
            int index = 0;
            int maxCoef = 0;
            Photo toCompare = a[indexToSearch];

            for (int i = 0; i < a.Count; i++)
            {
                if (i != indexToSearch && coef(toCompare, a[i]) > maxCoef)
                {
                    maxCoef = coef(toCompare, a[i]);
                    index = i;
                }
            }

            return index;
        }

        public static int searchWorstCoefV(List<Photo> a, int indexToSearch)
        {
            int index = 0;
            int minCoef = 999999999;
            Photo toCompare = a[indexToSearch];

            for (int i = 0; i < a.Count; i++)
            {
                if (i != indexToSearch && a[i].orientation == 'V' && coef(toCompare, a[i]) <= minCoef)
                {
                    minCoef = coef(toCompare, a[i]);
                    index = i;
                }
            }

            return index;
        }

        public static int searchBestCoefSlide(Slide b, List<Photo> a, int tt)
        {
            int index = 0;
            int maxCoef = 0;
           

            for (int i = 0; i < a.Count; i++)
            {
                if (tt != i && coefSlide(b, a[i]) > maxCoef)
                {
                    maxCoef = coefSlide(b, a[i]);
                    index = i;
                }
            }

            return index;
        }

        public static int coefSlide(Slide s, Photo p)
        {
            int tag_commun = 0;

            foreach (string tag1 in s.tags)
            {
                foreach (string tag2 in p.tags)
                {
                    if (tag1 == tag2)
                    {
                        tag_commun++;
                        break;
                    }
                }
            }

            return tag_commun;
        }
    }
}

