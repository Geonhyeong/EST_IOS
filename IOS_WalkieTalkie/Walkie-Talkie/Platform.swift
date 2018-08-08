//
//  Platform.swift
//  Walkie-Talkie
//

import Foundation

struct Platform {
    
    static var isSimulator: Bool {
        return TARGET_OS_SIMULATOR != 0 // Use this line in Xcode 7 or newer
        return TARGET_IPHONE_SIMULATOR != 0 // Use this line in Xcode 6
    }
    
}
