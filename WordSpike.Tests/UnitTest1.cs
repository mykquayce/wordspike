using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace WordSpike.Tests;

public class UnitTest1
{
	[Fact]
	public void PdfSharpTests2()
	{
		var document = new PdfDocument();

		var page = new PdfPage
		{
			Size = PdfSharpCore.PageSize.A4,
			Orientation = PdfSharpCore.PageOrientation.Landscape,
		};

		document.AddPage(page);

		var graphics = XGraphics.FromPdfPage(page);

		graphics.DrawImage(
			image: XImage.FromFile(@"E:\source\repos\mykquayce\WordSpike\WordSpike.Tests\Data\logo.png"),
			x: 0, y: 0, width: 200, height: 50);

		var font = new XFont("Consolas", 14, XFontStyle.BoldItalic);

		graphics.DrawString("4th Utility – Service Handover Form", font, XBrushes.White, x: 0, y: 50);

		document.Save("helloworld.pdf");
	}

	[Theory]
	[InlineData("Value 1", "Value 2", "Value 3", "Value 4")]
	public void PdfSharpTests_GenerateFromHtml(string value1, string value2, string value3, string value4)
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);

		Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

		var html = File.ReadAllText(path: Path.Combine(".", "Data", "template.html"))
			.Replace("{{long-html-date}}", today.ToLongHtmlDateString())
			.Replace("{{value-1}}", value1)
			.Replace("{{value-2}}", value2)
			.Replace("{{value-3}}", value3)
			.Replace("{{short-date}}", today.ToShortDateString())
			.Replace("{{value-4}}", value4);

		var config = new PdfGenerateConfig
		{
			PageOrientation = PdfSharp.PageOrientation.Landscape,
			PageSize = PdfSharp.PageSize.Foolscap,
		};

		var pdf = PdfGenerator.GeneratePdf(html, config, imageLoad: LoadImage);

		pdf.Save("helloworld.pdf");
	}
	private static void LoadImage(object? sender, TheArtOfDev.HtmlRenderer.Core.Entities.HtmlImageLoadEventArgs args)
	{
		var path = Path.Combine(".", "Data", args.Src);
		var image = PdfSharp.Drawing.XImage.FromFile(path);
		args.Callback(image);
	}

	[Theory]
	[InlineData("2023-09-29", "29<sup>th</sup> September 2023")]
	[InlineData("2023-09-30", "30<sup>th</sup> September 2023")]
	[InlineData("2023-10-01", "1<sup>st</sup> October 2023")]
	[InlineData("2023-10-02", "2<sup>nd</sup> October 2023")]
	[InlineData("2023-10-03", "3<sup>rd</sup> October 2023")]
	[InlineData("2023-10-04", "4<sup>th</sup> October 2023")]
	public void GetLongHtmlDateTests(string dateString, string expected)
	{
		var date = DateOnly.Parse(dateString);
		var actual = date.ToLongHtmlDateString();
		Assert.Equal(expected, actual);
	}
}

public static class SystemExtensions
{
	public static string ToLongHtmlDateString(this DateOnly date)
	{
		var daySuffix = date.Day switch
		{
			1 or 21 or 31 => "st",
			2 or 22 => "nd",
			3 or 23 => "rd",
			_ => "th",
		};

		return string.Concat(date.Day,
			"<sup>",
			daySuffix,
			"</sup> ",
			date.ToString("MMMM yyyy"));
	}
}
