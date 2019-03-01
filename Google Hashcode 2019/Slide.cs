using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google_Hashcode_2019
{
    class Slide
    {
        public List<Photo> photos = new List<Photo>();
        public List<string> tags;

        public void concatTagsPhotos()
        {
            List<string> a = new List<string>();
            foreach (Photo p in photos)
            {
                a.Union(p.tags);
            }

            tags = a;
        }
    }
}
