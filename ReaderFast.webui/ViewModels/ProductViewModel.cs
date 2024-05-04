namespace ReaderFast.webui.ViewModels
{ 
public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    // Eğer ek bilgiler taşınacaksa burada tanımlanabilir
}
}