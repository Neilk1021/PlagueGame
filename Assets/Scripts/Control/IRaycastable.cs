namespace RPG.Control{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingCon);
        CursorType GetCursorType();

    }
}
