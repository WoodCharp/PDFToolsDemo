using PDFToolsDemo;
using System.Diagnostics;
using TestConsoleApp;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;

string file = @$"{AppDomain.CurrentDomain.BaseDirectory}TestPDF.pdf";
string fontFile = @$"{AppDomain.CurrentDomain.BaseDirectory}Resources\fira-sans.regular.ttf";
string imageFile = @$"{AppDomain.CurrentDomain.BaseDirectory}Resources\TestImage.png";
string imageFile2 = @$"{AppDomain.CurrentDomain.BaseDirectory}Resources\Cog.png";

Console.WriteLine("Press any key to create test PDF file to");
Console.WriteLine(file);
Console.ReadKey();

double headerBlockHeight = 60;
double footerBlockHeight = 30;

MyPDFSettings pdfSettings = new MyPDFSettings(new PDFPadding(15, 15, 20, 20), 5, PageSize.A4, PdfAStandard.A2A, true);

MyPDF pdf = new MyPDF(pdfSettings, PDFTools.LoadFile(fontFile));
pdf.HeaderBlock = Test.HeaderBlock(pdf, headerBlockHeight, imageFile, 45, 45);
pdf.FooterBlock = Test.FooterBlock(pdf, footerBlockHeight);

pdf.Blocks.Add(Test.TitleBlock(pdf, "Some title text"));
pdf.Blocks.Add(Test.TestTable(pdf, imageFile2));
pdf.Blocks.Add(Test.TitleBlock(pdf, "Some title text"));
pdf.Blocks.Add(Test.TestTable(pdf, imageFile2));
pdf.Blocks.Add(Test.TitleBlock(pdf, "Some title text"));
pdf.Blocks.Add(Test.TestTable(pdf, imageFile2));
pdf.Blocks.Add(Test.TitleBlock2(pdf, "Some title text"));

pdf.BuildDocument();
pdf.SaveDocument(file);

Console.WriteLine("Created, press any key to open the PDF and exit the application.");
Console.ReadKey();

if (File.Exists(file)) Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });