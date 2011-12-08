
//
// Form to encapsulate the histogram control.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoPlayer
{
    public partial class VPHistogramForm : Form
    {
        public VPHistogramForm()
        {
            InitializeComponent();
        }

        public void SetValues(long[] values)
        {
            histogramControl.DrawHistogram(values);
        }
    }
}
