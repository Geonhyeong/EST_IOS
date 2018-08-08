//
//  Protocal.swift
//  GroupVoiceApp
//


import Foundation

class Header {
    
    var Command : String
    var Id_Length : Int
    var Id : String
    var Data_Length : Int
    
    static let RegisterCommand = "REG:"
    static let CancelCommand = "CAN:"
    static let VoiceCommand = "VOI:"
//    static let RefusedCommand = "REF"
    
    init(command : String = "" , idlength : Int = 0 , id : String = "", datalength : Int = 0) {
        Command = command
        Id_Length = idlength
        Id = id
        Data_Length = datalength
    }
    
    func intTOByte(int : Int) -> [UInt8] {
        var bytearray : [UInt8] = [0,0,0,0]
        bytearray[0] = UInt8(int>>24 & 0xff)
        bytearray[1] = UInt8(int>>16 & 0xff)
        bytearray[2] = UInt8(int>>8 & 0xff)
        bytearray[3] = UInt8(int & 0xff)
        
        return bytearray
    }
    
    func toByteArray () -> [UInt8]{
        var data = [UInt8](Command.utf8)
        data.append(contentsOf: intTOByte(int: Id_Length))
        data.append(contentsOf: [UInt8](Id.utf8))
        data.append(contentsOf: intTOByte(int: Data_Length))
        
        return data
    }

    
}
