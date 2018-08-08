//
//  RoundedCornerView.swift
//  Walkie-Talkie
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

