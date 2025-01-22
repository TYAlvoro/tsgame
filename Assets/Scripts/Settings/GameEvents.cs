using System;
using UnityEngine;

public static class GameEvents
{
    // События для передачи целей
    public static Action<ResourceType, Vector3> OnTargetAssigned;
    public static Action<ResourceType, Vector3> OnResourceCollected;
    public static Action<ResourceType> OnResourceDelivered;

    // Новые события для обновления UI счетчиков
    public static Action<ResourceType, int> OnResourceCountUpdated; // Обновление ресурсов
    public static Action<int> OnProductCountUpdated; // Обновление товаров
}