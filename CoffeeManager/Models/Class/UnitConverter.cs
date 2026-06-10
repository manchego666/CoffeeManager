using CoffeeManager.Models.Enums;

public static class UnitConverter
{
    public static decimal ToBase(UnitType unit, decimal value)
    {
        return unit switch
        {
            UnitType.Gramos => value,
            UnitType.Kilogramos => value * 1000m,

            UnitType.Mililitros => value,
            UnitType.Litros => value * 1000m,

            UnitType.Piezas => value,
            UnitType.Paquete => value,   // InventoryItem se encarga
            UnitType.Caja => value,

            _ => value
        };
    }

    public static decimal Convert(UnitType from, UnitType to, decimal value)
    {
        decimal baseValue = ToBase(from, value);

        return to switch
        {
            UnitType.Gramos => baseValue,
            UnitType.Kilogramos => baseValue / 1000m,

            UnitType.Mililitros => baseValue,
            UnitType.Litros => baseValue / 1000m,

            UnitType.Piezas => baseValue,
            UnitType.Paquete => baseValue,
            UnitType.Caja => baseValue,

            _ => baseValue
        };
    }
}
