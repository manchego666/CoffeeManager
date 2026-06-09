// ===============================================================
//  ZORRODEV 2026 — Notification Engine
//  Authors: Christopher (≧◡≦), Daniel (ง'̀-'́)ง, Brayan (✧ω✧), Jesús (◕‿◕✿)
//  Description: Centralized notification system for all modules.
// ===============================================================

using System;
using System.Collections.Generic;
using CoffeeManager.Models.Class;
using CoffeeManager.Services;

namespace CoffeeManager.Services.Logic
{
    /// <summary>
    /// Handles creation, loading and saving of system notifications.
    /// Used by all modules (Users, Products, Inventory, Sales, etc.). (◕‿◕✿) ZORRODEV2026
    /// </summary>
    public class NotificationService
    {
        private readonly JsonService _json = new();

        // TODO: move to PathService if you want central paths later.
        private const string FilePath = "Data/notifications.json";

        /// <summary>
        /// Loads all notifications from JSON.
        /// Returns an empty list if file does not exist. (≧◡≦)
        /// </summary>
        public List<Notification> LoadNotifications()
        {
            var list = _json.Load<Notification>(FilePath);
            return list ?? new List<Notification>();
        }



        /// <summary>
        /// Saves the full notification list. (✧ω✧)
        /// </summary>
        public void SaveNotifications(List<Notification> notifications)
        {
            _json.Save(FilePath, notifications);
        }

        /// <summary>
        /// Adds a new notification to the system and persists it. (◕‿◕✿)
        /// </summary>
        public void AddNotification(string title, string detail, string type = "info")
        {
            var list = LoadNotifications();

            list.Insert(0, new Notification
            {
                Title = title,
                Detail = detail,
                Date = DateTime.Now,
                Type = type
            });

            SaveNotifications(list);
        }

        // === HIGH-LEVEL HELPERS (Team ZORRODEV 2026) ===

        public void NotifyUserCreated(string name)
            => AddNotification("Usuarios", $"Nuevo usuario registrado: {name}", "success");

        public void NotifyUserUpdated(string name)
            => AddNotification("Usuarios", $"Usuario actualizado: {name}", "info");

        public void NotifyProductCreated(string name)
            => AddNotification("Productos", $"Nuevo producto agregado: {name}", "success");

        public void NotifyProductUpdated(string name)
            => AddNotification("Productos", $"Producto actualizado: {name}", "info");

        public void NotifyLowStock(string name, decimal qty)
            => AddNotification("Stock", $"El producto '{name}' está bajo en inventario ({qty}).", "warning");

        public void NotifyOutOfStock(string name)
            => AddNotification("Stock", $"El producto '{name}' se ha agotado.", "danger");
    }
}
