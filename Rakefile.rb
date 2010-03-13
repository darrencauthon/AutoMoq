# Rakefile.rb
require 'rubygems'      
require 'ftools'

ILMERGE = "ilmerge"
SOLUTION_ROOT = Dir.pwd
DEPLOY_DIRECTORY = "#{SOLUTION_ROOT}\\Deploy"
BIN_DIRECTORY = "#{SOLUTION_ROOT}\\AutoMoq/AutoMoq/bin/Debug"
DEPLOY_FILE =  "#{DEPLOY_DIRECTORY}\\AutoMoq.dll"

task :default=>[:say_hi]

desc "Says Hi"
task :say_hi do 
	puts "Hi there. Rake is working."
	puts "Type rake --tasks."
end

desc "Count files of each type in this solution"
task :file_count do
	report = {}
	Dir['./**/**'].each do |f| 
		report[File.extname(f)].nil? ? report[File.extname(f)] = 1 : report[File.extname(f)] += 1 
	end
	report.each do |type, count|
		puts "#{type}: #{count}"
	end   
end

desc "Merges the assemblies"
task :merge do
	mkdir "#{DEPLOY_DIRECTORY}" unless File.exists?(DEPLOY_DIRECTORY)
	includes = []
	Dir["#{BIN_DIRECTORY}/*.dll"].each do |f| 
		if (File.extname(f) == '.dll')
			includes.push f
		end
	end
	sh "#{ILMERGE} /t:library /out:#{DEPLOY_FILE} #{includes.join(" ")}"
end