using System.Collections.Generic;
using System.Threading.Tasks;

namespace MC_Suite.Serial
{
    public interface ISerialDeviceManager
    {
        Task<ISerialDevice> OpenByDeviceId(string deviceId);
        Task<IEnumerable<DeviceNode>> GetDeviceList();
    }
}
