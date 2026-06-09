// ===============================================================
//  ZORRODEV 2026 — Notification Model
//  Authors: Christopher (≧◡≦), Daniel (ง'̀-'́)ง, Brayan (✧ω✧), Jesús (◕‿◕✿)
//  Description: Simple JSON‑friendly notification entity.
// ===============================================================

using System;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a system notification displayed in the dashboard.
    /// This model is intentionally simple and JSON‑friendly. (≧◡≦) ZORRODEV2026
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Short category or label of the notification.
        /// Example: "Stock", "System", "Attendance", "Usuarios". (✧ω✧)
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed message describing the event. (◕‿◕✿)
        /// </summary>
        public string Detail { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of when the notification occurred. (≧◡≦)
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Optional type identifier for UI styling or filtering.
        /// Example: "info", "success", "warning", "danger". (ง'̀-'́)ง
        /// </summary>
        public string Type { get; set; } = "info";
    }
}
