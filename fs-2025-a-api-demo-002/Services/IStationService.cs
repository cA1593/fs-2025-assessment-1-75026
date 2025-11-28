using fs_2025_assessment_1_75026.Models;

namespace fs_2025_assessment_1_75026.Services
{
    public interface IStationService
    {
        List<Station> GetAllStations();
        Station? GetStationByNumber(int number);

        // Needed so the background service can save updated values
        void SaveAllStations(List<Station> stations);
    }
}
