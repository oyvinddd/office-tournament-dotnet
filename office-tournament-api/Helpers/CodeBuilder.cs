using System.Text;

namespace office_tournament_api.Helpers
{
    public class CodeBuilder
    {
        public CodeBuilder() { }

        public string RandomPassword()
        {
            string newCode = "";
            int length = 6;

            // creating a StringBuilder object()
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                stringBuilder.Append(letter);
            }
           
           newCode = stringBuilder.ToString(); 
           return newCode;
        }
    }
}

