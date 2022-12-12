using Messier.Interfaces;

namespace Messier.Time;

public class Clock : IClock
{
    public DateTime Current()
        => DateTime.UtcNow;
}