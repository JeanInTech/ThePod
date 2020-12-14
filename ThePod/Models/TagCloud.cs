using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class TagCloud
    {
        public static string MakeTagCloud(List<UserProfile> up)
        {
            Decimal totaltags = up.Count();
            Decimal tagpercent = 0;
            int tagweight = 0;

            StringBuilder TagCloud = new StringBuilder();

            var groupedtags = up.GroupBy(t => t.Tag);

            TagCloud.Append("");
            foreach (var tag in groupedtags)
            {
                if (tag != null)
                {
                    tagpercent = (tag.Count() / totaltags) * 100;

                    if (tagpercent >= 90)
                    {
                        tagweight = 1;
                    }
                    else if (tagpercent >= 70)
                    {
                        tagweight = 2;
                    }
                    else if (tagpercent >= 40)
                    {
                        tagweight = 3;
                    }
                    else if (tagpercent >= 20)
                    {
                        tagweight = 4;
                    }
                    else if (tagpercent >= 3)
                    {
                        tagweight = 5;
                    }
                    else
                    {
                        tagweight = 0;
                    }
                    TagCloud.Append(String.Format("{2} ", tag.Key.Replace(" ", "-"), tagweight, tag.Key));
                }
            }
            TagCloud.Append("");
            return TagCloud.ToString();
        }
    }
}
