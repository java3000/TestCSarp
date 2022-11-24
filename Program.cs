using FileLib;

/*
 * Результат может быть представлен в виде списка или дерева с дополнительной статистической информацией в разрезе группировки по MimeType:
Как часто встречается данный тип относительно всей собранной коллекции (количественное и процентное отношение)
 */
namespace TestCSarp
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemHelper fsh = new FileSystemHelper();
            var result = fsh.Scan(null);
            HtmlHelper.ProduceHtml("", result);
        }
    }
}