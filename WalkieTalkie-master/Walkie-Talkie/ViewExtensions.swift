//
//  UIViewExtensions.swift
//  RealEstate
//
//  Created by GeonHyeong on 18.08.22.
//  Copyright © 2018 GeonHyeong. All rights reserved.
//

import Foundation
import UIKit

extension UIView {
    
    var width:CGFloat{
        set{
            self.frame.size.width = newValue
        }
        get{
            return self.frame.size.width
        }
    }
    
    var height:CGFloat{
        set{
            self.frame.size.height = newValue
        }
        get{
            return self.frame.size.height
        }
    }
    
    var size:CGSize{
        set{
            self.frame.size = newValue
        }
        get{
            return self.frame.size
        }
    }
    
    var origin:CGPoint{
        set{
            self.frame.origin = newValue
        }
        get{
            return self.frame.origin
        }
    }
    
    
    var x:CGFloat{
        set{
            self.frame.origin.x = newValue
        }
        get{
            return self.frame.origin.x
        }
    }
    
    var y:CGFloat{
        set{
            self.frame.origin.y = newValue
        }
        get{
            return self.frame.origin.y
        }
    }
}

