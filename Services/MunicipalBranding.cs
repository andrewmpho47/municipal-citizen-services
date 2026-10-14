using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MunicipalReporter.Services
{
    public static class MunicipalBranding
    {
        public static PictureBox CreateBannerPictureBox(int width, int height)
        {
            string imagePath = Path.Combine(
                Application.StartupPath,
                "Assets",
                "MunicipalCitizensServicesBanner.png");

            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException(
                    "The municipal services banner was not found. Ensure the Assets folder is copied beside the application.",
                    imagePath);
            }

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(width, height);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.FromArgb(0, 91, 166);
            pictureBox.AccessibleName = "Municipal Citizens Services banner";
            pictureBox.Disposed += delegate
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
            };

            using (Image sourceImage = Image.FromFile(imagePath))
            {
                pictureBox.Image = new Bitmap(sourceImage);
            }

            return pictureBox;
        }
    }
}
