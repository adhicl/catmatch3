using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class LeaderboardPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtScore;

        public void SetPanel(int position, string playerName, float score)
        {
            txtNumber.text = (position + 1).ToString() + ".";
            txtName.text = playerName.Truncate(20);
            txtScore.text = score.ToString("N0");
        }
    }
}