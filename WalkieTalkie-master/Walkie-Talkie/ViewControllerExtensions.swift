//
//  ViewControllerExtensions.swift
//  Walkie-Talkie
//
//  Created by GeonHyeong on 18.08.22.
//  Copyright © 2018 GeonHyeong. All rights reserved.
//

import Foundation
import UIKit

extension UIViewController{
    
    func showAlert(title:String, mess:String, completion:((UIAlertAction)->())?=nil)
    {
        let aVC = UIAlertController(title: title, message: mess, preferredStyle: .alert)
        aVC.addAction(UIAlertAction(title: "OK", style: .default, handler:completion))
        self.present(aVC, animated: true, completion: nil)
    }
}
