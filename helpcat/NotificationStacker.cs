using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helpcat
{
    static class NotificationStacker
    {
        static List<int> usedSlots = new List<int>();

        public static int ReserveSlot()
        {
            int slot = 0;
            lock (usedSlots)
            {
                while (usedSlots.Contains(slot))
                    slot++;
                usedSlots.Add(slot);
            }
            return slot;
        }

        public static void CancelSlot(int slot)
        {
            lock (usedSlots)
            {
                if (usedSlots.Contains(slot))
                    usedSlots.Remove(slot);
            }
        }
    }
}
