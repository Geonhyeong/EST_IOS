//
//  Platform.swift
//  Walkie-Talkie
//
//  Created by GeonHyeong on 18.08.22.
//  Copyright © 2018 GeonHyeong. All rights reserved.
//

import Foundation

struct Platform {
    
    static var isSimulator: Bool {
        return TARGET_OS_SIMULATOR != 0 // Use this line in Xcode 7 or newer
        return TARGET_IPHONE_SIMULATOR != 0 // Use this line in Xcode 6
    }
    
}
