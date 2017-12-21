package com.impinker.impinkerai.controller;


import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class SolrSearchController {

    @GetMapping("index")
    public String test(){

        return  "search index";
    }
}
