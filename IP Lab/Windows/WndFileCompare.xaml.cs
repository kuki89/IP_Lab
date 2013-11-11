using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.IO;
using System.Runtime.InteropServices;

using LIMS.Analysis.Helpers;

namespace IP_Lab
{
    public partial class WndFileCompare : ChildWindow
    {
        private string strSrc = "";
        private string strDst = "";

        public WndFileCompare()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            rtbFile1.Blocks.Clear();
            rtbFile2.Blocks.Clear();
        }

        private void btnFile1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog { Multiselect = false };
                if (dialog.ShowDialog() == true)
                {
                    using (var stream = dialog.File.OpenRead())
                    {
                        StreamReader sr = new StreamReader(stream, new Gb2312Encoding());
                        strSrc = sr.ReadToEnd();

                        // 删除多余的换行
                        strSrc = strSrc.Replace("\0", "");

                        if (!string.IsNullOrEmpty(strSrc))
                            CompareFile();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnFile2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog { Multiselect = false };
                if (dialog.ShowDialog() == true)
                {
                    using (var stream = dialog.File.OpenRead())
                    {
                        StreamReader sr = new StreamReader(stream, new Gb2312Encoding());
                        strDst = sr.ReadToEnd();

                        // 删除多余的换行
                        strDst = strDst.Replace("\0", "");

                        if (!string.IsNullOrEmpty(strDst))
                            CompareFile();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CompareFile()
        {
            Diff.Item[] f = Diff.DiffText(strSrc, strDst, false, false, false);
            string[] split = { System.Environment.NewLine };
            string[] srcLines = strSrc.Split(split, StringSplitOptions.None);
            string[] dstLines = strDst.Split(split, StringSplitOptions.None);

            rtbFile1.Blocks.Clear();
            rtbFile2.Blocks.Clear();

            int n = 0;
            for (int fdx = 0; fdx < f.Length; fdx++)
            {
                Diff.Item srcItem = f[fdx];

                // 相同的行
                while ((n < srcItem.StartB) && (n < dstLines.Length))
                {
                    Paragraph prgParagraph1 = new Paragraph();
                    Run rnMyText1 = new Run();
                    rnMyText1.Text = dstLines[n] + System.Environment.NewLine;
                    prgParagraph1.Inlines.Add(rnMyText1);

                    Paragraph prgParagraph2 = new Paragraph();
                    Run rnMyText2 = new Run();
                    rnMyText2.Text = dstLines[n] + System.Environment.NewLine;
                    prgParagraph2.Inlines.Add(rnMyText2);

                    rtbFile1.Blocks.Add(prgParagraph1);
                    rtbFile2.Blocks.Add(prgParagraph2);

                    n++;
                }

                // A有，B没有的行
                for (int m = 0; m < srcItem.deletedA; m++)
                {
                    Paragraph prgParagraph = new Paragraph();
                    Run rnMyText = new Run();
                    rnMyText.Text = srcLines[srcItem.StartA + m] + System.Environment.NewLine;
                    rnMyText.Foreground = new SolidColorBrush(Colors.Red);
                    prgParagraph.Inlines.Add(rnMyText);

                    // 空格
                    Paragraph paragraph = new Paragraph();
                    Run whiteText = new Run();
                    whiteText.Text = System.Environment.NewLine;
                    paragraph.Inlines.Add(whiteText);

                    rtbFile1.Blocks.Add(prgParagraph);
                    rtbFile2.Blocks.Add(paragraph);
                }

                // A没有，B有的行
                while (n < srcItem.StartB + srcItem.insertedB)
                {
                    Paragraph prgParagraph = new Paragraph();
                    Run rnMyText = new Run();
                    rnMyText.Text = dstLines[n] + System.Environment.NewLine;
                    rnMyText.Foreground = new SolidColorBrush(Colors.Red);
                    prgParagraph.Inlines.Add(rnMyText);

                    // 空格
                    Paragraph paragraph = new Paragraph();
                    Run whiteText = new Run();
                    whiteText.Text = System.Environment.NewLine;
                    paragraph.Inlines.Add(whiteText);

                    rtbFile1.Blocks.Add(paragraph);
                    rtbFile2.Blocks.Add(prgParagraph);
                    n++;
                }
            }

            // 剩余的相同的行
            while (n < dstLines.Length)
            {
                Paragraph prgParagraph1 = new Paragraph();
                Run rnMyText1 = new Run();
                rnMyText1.Text = dstLines[n] + System.Environment.NewLine;
                prgParagraph1.Inlines.Add(rnMyText1);

                Paragraph prgParagraph2 = new Paragraph();
                Run rnMyText2 = new Run();
                rnMyText2.Text = dstLines[n] + System.Environment.NewLine;
                prgParagraph2.Inlines.Add(rnMyText2);

                rtbFile1.Blocks.Add(prgParagraph1);
                rtbFile2.Blocks.Add(prgParagraph2);

                n++;
            }
        }
    }
}

