package com.lang.factory;

import com.lang.autohome.AutoHomeXPathCommon;
import com.lang.bitauto.BitautoXPathCommon;
import com.lang.common.CompanyEnum;
import com.lang.fblife.FbLifeXPathCommon;
import com.lang.interfac.MotorXPathInterface;

public class XPathFactory {

	public MotorXPathInterface createXPath(CompanyEnum compaType) {
		switch (compaType) {

		case Fblife:
			return new FbLifeXPathCommon();

		case Autohome:
			return new AutoHomeXPathCommon();

		case Yiche:
			return new BitautoXPathCommon();
		default:
			break;
		}
		return null;
	}
}
