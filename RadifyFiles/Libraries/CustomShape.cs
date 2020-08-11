using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace PGTAWPF
{
    public class CustomShape : Shape
    {
        public int ID;
        public CustomShape(int id)
            :base()
        {
            this.ID = id;
        }

        protected override Geometry DefiningGeometry => throw new NotImplementedException();
    }
}
