using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GUI_Final
{
    public class Pages
    {
        private static Pages instance;

        // Public property to get the singleton instance.
        public static Pages PagesInstance
        {
            get { return instance ?? (instance = new Pages()); }
        }

        // Private constructor to prevent others from making a new one.
        private Pages()
        {
            pages = new List<List<PageShapes>>(); // List of lists since we must be able to keep track of the shapes *on each page*
            undoes = new List<PageShapes>(); // Stack of lists for handling undo functionality. Can just add a second one for redo, but one thing at a time here.
            redoes = new List<PageShapes>();
            currentPage = 0;
        }

        // The actual reason this class was made.
        public List<List<PageShapes>> pages;
        public List<PageShapes> undoes;
        public List<PageShapes> redoes;
        public int currentPage;
    }
}
