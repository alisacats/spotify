using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.SpotifyWebInteraction
{
    public static class SpotifyContextMenu
    {
        static async Task RightClick(Page page, string selector)
        {
            await page.EvaluateExpressionAsync(@"(() => {
function rightClick(element) {
    const e = element.ownerDocument.createEvent('MouseEvents');

    e.initMouseEvent(
         'contextmenu', true, true,
         element.ownerDocument.defaultView, 1, 0, 0, 0, 0, false,
         false, false, false,2, null
    );

    return !element.dispatchEvent(e);
}

const elm = document.querySelector(`" + selector + @"`);
rightClick(elm);
})()");
        }

        public static async Task OpenContextMenuForTheFirstSong(Page page)
        {
            await page.WaitForSelectorAsync(".tracklist-name");
            await RightClick(page, ".tracklist-name");
            await page.WaitForSelectorAsync(".react-contextmenu--visible");
        }
    }
}
