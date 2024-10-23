namespace OxLibrary
{
    public class OxIcons
    {
        private static readonly System.Resources.ResourceManager resourceMan = new("OxLibrary.Properties.Resources", typeof(Properties.Resources).Assembly);

        internal OxIcons()
        {
        }

        public static System.Resources.ResourceManager ResourceManager => resourceMan;

        private static Bitmap GetBitmap(string name)
        {
            byte[] byteArrayIn = (byte[])ResourceManager.GetObject(name)!;
            using var ms = new MemoryStream(byteArrayIn);
            return new Bitmap(ms);
        }

        public static Bitmap Account => GetBitmap("account");
        public static Bitmap Batch_edit => GetBitmap("batch_edit");
        public static Bitmap BronzeTrophy => GetBitmap("bronzeTrophy");
        public static Bitmap Close => GetBitmap("close");
        public static Bitmap Console => GetBitmap("console");
        public static Bitmap Copy => GetBitmap("copy");
        public static Bitmap Cross => GetBitmap("cross");
        public static Bitmap Dlc => GetBitmap("dlc");
        public static Bitmap DoubleTick => GetBitmap("double_tick");
        public static Bitmap Down => GetBitmap("down");
        public static Bitmap Dualshock => GetBitmap("dualshock");
        public static Bitmap Elipsis => GetBitmap("elipsis");
        public static Bitmap Eraser => GetBitmap("eraser");
        public static Bitmap Error => GetBitmap("error");
        public static Bitmap Export => GetBitmap("export");
        public static Bitmap Eye => GetBitmap("eye");
        public static Bitmap Field => GetBitmap("field");
        public static Bitmap First => GetBitmap("first");
        public static Bitmap Folder => GetBitmap("folder");
        public static Bitmap Game => GetBitmap("game");
        public static Bitmap General => GetBitmap("general");
        public static Bitmap Go => GetBitmap("go");
        public static Bitmap GoldTrophy => GetBitmap("goldTrophy");
        public static Bitmap Info => GetBitmap("info");
        public static Bitmap Install => GetBitmap("install");
        public static Bitmap Installation => GetBitmap("installation");
        public static Bitmap Last => GetBitmap("last");
        public static Bitmap Left => GetBitmap("left");
        public static Bitmap Link => GetBitmap("link");
        public static Bitmap LinkedItems => GetBitmap("linkedItems");
        public static Bitmap Key => GetBitmap("key");
        public static Bitmap Maximize => GetBitmap("maximize");
        public static Bitmap Minimize => GetBitmap("minimize");
        public static Bitmap Minus => GetBitmap("minus");
        public static Bitmap NotEqual => GetBitmap("not_equal");
        public static Bitmap Pencil => GetBitmap("pencil");
        public static Bitmap Pin => GetBitmap("pin");
        public static Bitmap PlatinumTrophy => GetBitmap("platinumTrophy");
        public static Bitmap Plus => GetBitmap("plus");
        public static Bitmap PlusThick => GetBitmap("plus-thick");
        public static Bitmap PS1 => GetBitmap("ps1");
        public static Bitmap PS2 => GetBitmap("ps2");
        public static Bitmap PS3 => GetBitmap("ps3");
        public static Bitmap PS4 => GetBitmap("ps4");
        public static Bitmap PS5 => GetBitmap("ps5");
        public static Bitmap PSP => GetBitmap("psp");
        public static Bitmap PSVita => GetBitmap("psvita");
        public static Bitmap Related => GetBitmap("related");
        public static Bitmap Replace => GetBitmap("replace");
        public static Bitmap Restore => GetBitmap("restore");
        public static Bitmap Question => GetBitmap("question");
        public static Bitmap Right => GetBitmap("right");
        public static Bitmap Save => GetBitmap("save");
        public static Bitmap Select => GetBitmap("select");
        public static Bitmap Series => GetBitmap("series");
        public static Bitmap Settings => GetBitmap("settings");
        public static Bitmap Share => GetBitmap("share");
        public static Bitmap SilverTrophy => GetBitmap("silverTrophy");
        public static Bitmap Storage => GetBitmap("storage");
        public static Bitmap Synchronize => GetBitmap("synchronize");
        public static Bitmap Tag => GetBitmap("tag");
        public static Bitmap Tick => GetBitmap("tick");
        public static Bitmap Trash => GetBitmap("trash");
        public static Bitmap Unpin => GetBitmap("unpin");
        public static Bitmap Up => GetBitmap("up");
        public static Bitmap Warning => GetBitmap("warning");
    }
}
