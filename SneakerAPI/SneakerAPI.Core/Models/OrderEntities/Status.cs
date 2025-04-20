public enum OrderStatus
{
    Cancelled = 0,
    Pending = 1,
    Processing = 2,
    ReadyToPick = 3,
    Picking = 4,
    Picked = 5,
    Delivering = 6,
    Delivered = 7,
    Completed = 9,
    Other=400

}
public static class GHNStatusMapper
{
    public static OrderStatus MapToOrderStatus(string ghnStatus)
    {
        return ghnStatus switch
        {
            "ready_to_pick" => OrderStatus.ReadyToPick,
            "delivering"    => OrderStatus.Delivering,
            "delivered"     => OrderStatus.Delivered,
            "cancel"        => OrderStatus.Cancelled,
            "create"        => OrderStatus.Processing,
            "picking"   => OrderStatus.Picking,
            "picked" =>OrderStatus.Picked,
            _               => OrderStatus.Other
        };
    }
}


