//
//  RoundedCornerView.swift
//  Walkie-Talkie
//
//  Created by GeonHyeong on 18.08.22.
//  Copyright © 2018 GeonHyeong. All rights reserved.
//

import Foundation
import UIKit

extension UIView{
    
    @IBInspectable private var cornerRadius:CGFloat{
        set{
            self.layer.cornerRadius = CGFloat(newValue)
        }
        get{
            return self.layer.cornerRadius
        }
    }
    override open func awakeFromNib() {
        super.awakeFromNib()
        self.layer.masksToBounds = false
    }
}

