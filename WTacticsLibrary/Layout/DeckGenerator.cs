using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Layout
{
    public static class DeckGenerator
    {

        public static void GenerateDeck(Guid deckGuid)
        {
            var jsonFile = Repository.GetDeckJsonFile(deckGuid);
            if (!File.Exists(jsonFile)) return;

            var deck = JsonConvert.DeserializeObject<Deck>(File.ReadAllText(jsonFile));
            if (deck.DeckCards.Sum(x=>x.Quantity) > 75) return;

            using (var doc = new Document(PageSize.A4))
            {
                try
                {
                    var dpi = 600;

                    var height = doc.PageSize.Height;
                    var width = doc.PageSize.Width;

                    var dpiFactor = 72.0f/dpi;

                    var gap = Utilities.MillimetersToPoints(1);

                    var imageWidth = 1535*dpiFactor;
                    var startx = (width - 3*imageWidth - 2*gap) /2.0f;
                    var increasex = (imageWidth + gap);

                    var imageHeight = 2175*dpiFactor;
                    var starty = (height - 3*imageHeight - 2*gap) /2.0f;
                    var increasey = (imageHeight + gap);


                    PdfWriter.GetInstance(doc, new FileStream(Repository.GetDeckFile(deck.Guid), FileMode.Create));

                    doc.AddAuthor($"{deck.Creator.Name}");
                    doc.AddCreator($"{deck.Creator.Name} - wtactics.org");
                    doc.AddTitle($"{deck.Name} - wtactics.org");

                    doc.Open();

                    doc.Add(new Paragraph($"{deck.Name}") { SpacingAfter = 12, Font = { Size = 28 } });
                    doc.Add(new Paragraph($"By {deck.Creator.Name} - {deck.LastModifiedTime.Value.ToShortDateString()} - wtactics.org") { SpacingAfter = 24, Font = { Size = 16 } });
                    doc.Add(new Paragraph($"Cards List") { SpacingAfter = 12, Font = { Size = 28 } });
                    foreach (var deckCard in deck.DeckCards)
                    {
                        doc.Add(new Paragraph($"{deckCard.Quantity} x {deckCard.Card.Name}") { Font = { Size = 16 } });
                    }

                    doc.NewPage();
                    
                    var cardcounter = 0;
                    foreach (var deckCard in deck.DeckCards)
                    {
                        var cardPngFile = Repository.GetHighResolutionPngFile(deckCard.Card.Guid);
                        if (!File.Exists(cardPngFile)) continue;

                        Image png = Image.GetInstance(cardPngFile);
                        png.SetDpi(dpi, dpi);
                        png.ScaleAbsolute(imageWidth, imageHeight);

                        for (var i = 0; i < deckCard.Quantity; i++)
                        {

                          
                            var row = cardcounter/3;
                            var col = cardcounter%3;

                            var x = startx + col*increasex;
                            var y = starty + (2 - row)*increasey;

                            png.SetAbsolutePosition( x, y);
                            doc.Add(png);

                            cardcounter++;
                            if (cardcounter == 9)
                            {
                                doc.NewPage();
                                cardcounter = 0;
                            }
                        }
                    }

                    // start new page for back
                    if (cardcounter != 0)
                    {
                        doc.NewPage();
                    }

                    var cardBackPngFile = Repository.GetBackPngFile();
                    if (File.Exists(cardBackPngFile))
                    {
                        Image png = Image.GetInstance(cardBackPngFile);
                        png.SetDpi(dpi, dpi);
                        png.ScaleAbsolute(imageWidth, imageHeight);

                        for (var i = 0; i < 9; i++)
                        {
                            var row = i / 3;
                            var col = i % 3;

                            var x = startx + col * increasex;
                            var y = starty + (2 - row) * increasey;

                            png.SetAbsolutePosition(x, y);
                            doc.Add(png);
                        }
                    }

                   

                }
                catch (Exception ex)
                {

                }
            }

        }

    }


}
