package com.lang.fblife;

import java.util.Date;

import us.codecraft.webmagic.Spider;

import com.lang.util.TUtil;

public class FblifeMain {

	public static void main(String[] args) {
		TUtil.getUTCTime(new Date().getTime());
		TUtil.getBJUtcTime();

		// TourPageStart();

		// CulturePageStart();
	}

	public static void CulturePageStart() {
		Spider.create(new FblifeCulturePageProcessor())
				.addUrl("http://www.fblife.com/")
				.addPipeline(new FblifePipeline()).thread(5).run();
	}

	public static void TourPageStart() {
		Spider.create(new FbLifeTourPageProcessor())
				.addUrl("http://tour.fblife.com/")
				.addPipeline(new FblifePipeline()).thread(5).run();
	}

}
