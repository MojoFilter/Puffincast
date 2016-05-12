using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using Microsoft.Band.Personalization;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Puffincast.MicrosoftBand
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }
        public async void Stuff()
        {
            try
            {
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                IBandInfo band = pairedBands[0];

                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(band))
                {

                    await AddTileIfMissing(bandClient);


                }
            }
            catch (BandException ex)
            {
                var y = ex;
            }
        }
        internal enum TileLayoutIndex : int
        {
            MessagesLayout = 0
        }

        internal enum TileMessagesLayoutElementId : short
        {
            Message1 = 1,
            Message2 = 2
        }

        public async Task AddTileIfMissing(IBandClient bandClient)
        {
            try
            {
                IEnumerable<BandTile> tiles = await bandClient.TileManager.GetTilesAsync();

                var tileToRemove =
                    tiles.Where(p => p.Name == "Puffincast")
                         .Select(p => bandClient.TileManager.RemoveTileAsync(p))
                         .ToList();

                int tileCapacity = await bandClient.TileManager.GetRemainingTileCapacityAsync();

                if (tileCapacity > 0)
                {
                    var smallIconBitmap = new WriteableBitmap(24, 24);
                    var tileIconBitmap = new WriteableBitmap(46, 46);

                    var smallIcon = smallIconBitmap.ToBandIcon();
                    var tileIcon = tileIconBitmap.ToBandIcon();

                    var tileGuid = Guid.NewGuid();
                    var tile = new BandTile(tileGuid)
                    {
                        IsBadgingEnabled = true,
                        Name = "Puffincast",
                        SmallIcon = smallIcon,
                        TileIcon = tileIcon
                    };

                    await AddPanelToTile(bandClient, tile);
                }
            }
            catch (Exception ex)
            {
                var y = ex;
                throw;
            }
        }

        public async Task AddPanelToTile(IBandClient bandClient, BandTile tile)
        {
            try
            {
                var panel = new ScrollFlowPanel
                {
                    Rect = new PageRect(0, 0, 258, 128),
                    Orientation = FlowPanelOrientation.Horizontal,
                    ScrollBarColorSource = ElementColorSource.BandBase,
                };

                var textBlock = new Microsoft.Band.Tiles.Pages.TextBlock
                {
                    ElementId = (short)TileMessagesLayoutElementId.Message1,
                    Rect = new PageRect(0, 0, 258, 128),
                    Margins = new Margins(15, 0, 15, 0),
                    Color = new BandColor(0xFF, 0xFF, 0xFF)
                };
                var textBlockData = new TextBlockData((int)TileMessagesLayoutElementId.Message1, "This is a test message");

                var textButton = new TextButton()
                {
                    ElementId = 4,
                    HorizontalAlignment = Microsoft.Band.Tiles.Pages.HorizontalAlignment.Center,
                    Rect = new PageRect(0, 0, 100, 50),
                    VerticalAlignment = Microsoft.Band.Tiles.Pages.VerticalAlignment.Center,
                };

                var textButtonData = new TextButtonData(4, "Play");

                panel.Elements.Add(textBlock);
                panel.Elements.Add(textButton);

                var layout = new PageLayout(panel);
                tile.PageLayouts.Add(layout);

                await bandClient.TileManager.AddTileAsync(tile);

                var pageContent = new PageData(Guid.NewGuid(), (int)TileLayoutIndex.MessagesLayout, textBlockData, textButtonData);

                var pageSet = await bandClient.TileManager.SetPagesAsync(tile.TileId, pageContent);

            }
            catch (Exception ex)
            {
                var y = ex;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Stuff();
        }
    }
}

