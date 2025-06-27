using System.IO;

namespace wms_finish.Sqlite;

public static class FactoryExtension
{
    public static void initailize(this FactoryContext context)
    {
        if (File.Exists(context.DbFilePath) == false)
        {
            context.Database.EnsureCreated();

            // 재고 현황 예제값 추가
            context.Inventories.Add(new Inventory() { Id = 1, ItemName = "마우스", Quantity = 100, WhsName = "서울" });
            context.Inventories.Add(new Inventory() { Id = 2, ItemName = "키보드",  Quantity = 50, WhsName = "서울" });
            context.Inventories.Add(new Inventory() { Id = 3, ItemName = "모니터", Quantity = 10, WhsName = "서울" });
            context.Inventories.Add(new Inventory() { Id = 4, ItemName = "마우스", Quantity = 100, WhsName = "부산" });
            context.Inventories.Add(new Inventory() { Id = 5, ItemName = "키보드", Quantity = 100, WhsName = "대구" });

            context.SaveChanges();
        }
    }
}

