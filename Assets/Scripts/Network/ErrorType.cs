namespace Network   
{
    public enum ErrorType : ushort
    {
        Unknown = 0,
        NotValidNickname = 1,
        NotValidNameRoom = 2,
        MessageInRoomNotValid = 3,
        NotValidVersion = 4,
        RoomNameIsExists = 5,
        RoomIsFull = 6
    }
}
